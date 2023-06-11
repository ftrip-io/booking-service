using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.booking_service.contracts.ReservationRequests.Events;
using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.booking_service.Reservations.UseCases.CreateReservation;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.AcceptReservationRequests
{
    public class AcceptReservationRequestHandler : IRequestHandler<AcceptReservationRequest, ReservationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservationRequestRepository _reservationRequestRepository;
        private readonly IReservationRequestQueryHelper _reservationRequestQueryHelper;
        private readonly IMediator _mediator;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public AcceptReservationRequestHandler(
            IUnitOfWork unitOfWork,
            IReservationRequestRepository reservationRequestRepository,
            IReservationRequestQueryHelper reservationRequestQueryHelper,
            IMediator mediator,
            IMessagePublisher messagePublisher,
            IStringManager stringManager,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _reservationRequestRepository = reservationRequestRepository;
            _reservationRequestQueryHelper = reservationRequestQueryHelper;
            _mediator = mediator;
            _messagePublisher = messagePublisher;
            _stringManager = stringManager;
            _logger = logger;
        }

        public async Task<ReservationRequest> Handle(AcceptReservationRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var existingReservationRequest = await _reservationRequestQueryHelper.ReadOrThrow(request.ReservationRequestId, cancellationToken);
            Validate(existingReservationRequest);

            await AcceptRequest(existingReservationRequest, cancellationToken);

            await CreateReservation(existingReservationRequest, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            await PublishReservationRequestAcceptedEvent(existingReservationRequest, cancellationToken);

            return existingReservationRequest;
        }

        private void Validate(ReservationRequest reservationRequest)
        {
            if (!reservationRequest.CanBeModified)
            {
                _logger.Error(
                    "Reservation Request cannot be accepted because state is different from Waiting - RequestId[{RequestId}], Status[{Status}]",
                    reservationRequest.Id, reservationRequest.Status
                );
                throw new BadLogicException(_stringManager.Format("Request_Validation_InvalidStatus", reservationRequest.Id, "accepted"));
            }
        }

        private async Task<ReservationRequest> AcceptRequest(ReservationRequest request, CancellationToken cancellationToken)
        {
            request.Status = ReservationRequestStatus.Accepted;

            var acceptedRequest = await _reservationRequestRepository.Update(request, cancellationToken);

            _logger.Information("Reservation Request accepted - RequestId[{RequestId}]", acceptedRequest.Id);

            return acceptedRequest;
        }

        private async Task CreateReservation(ReservationRequest existingReservationRequest, CancellationToken cancellationToken)
        {
            var reservationRequest = new CreateReservationRequest()
            {
                GuestId = existingReservationRequest.GuestId,
                DatePeriod = new CreateDatePeriodRequest()
                {
                    DateFrom = existingReservationRequest.DatePeriod.DateFrom,
                    DateTo = existingReservationRequest.DatePeriod.DateTo
                },
                AccomodationId = existingReservationRequest.AccomodationId,
                GuestNumber = existingReservationRequest.GuestNumber
            };

            await _mediator.Send(reservationRequest, cancellationToken);
        }

        private async Task PublishReservationRequestAcceptedEvent(ReservationRequest request, CancellationToken cancellationToken)
        {
            var requestAccepted = new ReservationRequestAcceptedEvent()
            {
                RequestId = request.Id,
                AccomodationId = request.AccomodationId,
                GuestId = request.GuestId,
                From = request.DatePeriod.DateFrom,
                To = request.DatePeriod.DateTo,
            };

            await _messagePublisher.Send<ReservationRequestAcceptedEvent, string>(requestAccepted, cancellationToken);
        }
    }
}