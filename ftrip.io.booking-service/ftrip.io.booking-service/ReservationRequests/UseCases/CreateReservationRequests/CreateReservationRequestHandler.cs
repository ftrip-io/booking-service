using AutoMapper;
using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.booking_service.Common.Domain;
using ftrip.io.booking_service.contracts.ReservationRequests;
using ftrip.io.booking_service.contracts.ReservationRequests.Events;
using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.booking_service.Reservations;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Serilog;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.CreateReservationRequests
{
    public class CreateReservationRequestHandler : IRequestHandler<CreateReservationRequest, ReservationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservationRequestRepository _reservationRequestRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IAccommodationQueryHelper _accommodationQueryHelper;
        private readonly IMapper _mapper;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IStringManager _stringManager;
        private readonly ICatalogServiceClient _catalogServiceClient;
        private readonly ILogger _logger;

        public CreateReservationRequestHandler(
            IUnitOfWork unitOfWork,
            IReservationRequestRepository reservationRequestRepository,
            IReservationRepository reservationRepository,
            IAccommodationQueryHelper accommodationQueryHelper,
            IMapper mapper,
            IMessagePublisher messagePublisher,
            IStringManager stringManager,
            ICatalogServiceClient catalogServiceClient,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _reservationRequestRepository = reservationRequestRepository;
            _accommodationQueryHelper = accommodationQueryHelper;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _messagePublisher = messagePublisher;
            _stringManager = stringManager;
            _catalogServiceClient = catalogServiceClient;
            _logger = logger;
        }

        public async Task<ReservationRequest> Handle(CreateReservationRequest request, CancellationToken cancellationToken)
        {
            var accommodation = await _accommodationQueryHelper.ReadOrThrow(request.AccomodationId, cancellationToken);
            await Validate(request, cancellationToken);

            var priceInfo = await GetPriceInfoOrThrow(request);

            await _unitOfWork.Begin(cancellationToken);

            var reservationRequest = _mapper.Map<ReservationRequest>(request);
            reservationRequest.TotalPrice = priceInfo.TotalPrice;

            var createdReservationRequest = await CreateRequest(reservationRequest, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            await PublishReservationRequestCreatedEvent(accommodation.HostId, createdReservationRequest, cancellationToken);

            return createdReservationRequest;
        }

        private async Task Validate(CreateReservationRequest reservationRequest, CancellationToken cancellationToken)
        {
            var datePeriod = _mapper.Map<DatePeriod>(reservationRequest.DatePeriod);
            if (await _reservationRepository.HasAnyByAccomodationAndDatePeriod(reservationRequest.AccomodationId, datePeriod, cancellationToken))
            {
                _logger.Error("Accommodation already booked - From[{From}], To[{To}]", datePeriod.DateFrom, datePeriod.DateTo);
                throw new BadLogicException(_stringManager.GetString("Request_Validation_AlreadyBooked"));
            }
        }

        private async Task<ReservationRequest> CreateRequest(ReservationRequest request, CancellationToken cancellationToken)
        {
            request.Status = ReservationRequestStatus.Waiting;

            var createdRequest = await _reservationRequestRepository.Create(request, cancellationToken);

            _logger.Information(
                "Reservation Request created - RequestId[{RequestId}], GuestId[{GuestId}], AccommodationId[{AccommodationId}]",
                createdRequest.Id, createdRequest.GuestId, createdRequest.AccomodationId
            );

            return createdRequest;
        }
        private async Task<PriceInfo> GetPriceInfoOrThrow(CreateReservationRequest request) 
        {
            var priceInfo = await _catalogServiceClient.GetPriceInfo(request.AccomodationId, request.DatePeriod.DateFrom, request.DatePeriod.DateTo, request.GuestNumber);

            if (priceInfo.Problems.Any())
            {
                var message = string.Join("\n", priceInfo.Problems);
              
                _logger.Error("Accommodation cannot be booked: {message}", message);
                throw new BadLogicException(message);
            }

            return priceInfo;
        }
        private async Task PublishReservationRequestCreatedEvent(Guid hostId, ReservationRequest reservationRequest, CancellationToken cancellationToken)
        {
            var requestCreated = new ReservationRequestCreatedEvent()
            {
                ReservationRequestId = reservationRequest.Id,
                AccommodationId = reservationRequest.AccomodationId,
                HostId = hostId,
                GuestId = reservationRequest.GuestId,
                From = reservationRequest.DatePeriod.DateFrom,
                To = reservationRequest.DatePeriod.DateTo,
                TotalPrice = reservationRequest.TotalPrice
            };

            await _messagePublisher.Send<ReservationRequestCreatedEvent, string>(requestCreated, cancellationToken);
        }
    }
}