﻿using AutoMapper;
using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.booking_service.Common.Domain;
using ftrip.io.booking_service.contracts.ReservationRequests.Events;
using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.booking_service.Reservations;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.CreateReservationRequests
{
    public class CreateReservationRequestHandler : IRequestHandler<CreateReservationRequest, ReservationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservationRequestRepository _reservationRequestRepository;
        private readonly IAccommodationQueryHelper _accommodationQueryHelper;
        private readonly IReservationRepository _reservationRepository;
        private readonly IMapper _mapper;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IStringManager _stringManager;
       
        public CreateReservationRequestHandler(
            IUnitOfWork unitOfWork, 
            IReservationRequestRepository reservationRequestRepository,
            IReservationRepository reservationRepository,
            IAccommodationQueryHelper accommodationQueryHelper,
            IMapper mapper,
            IMessagePublisher messagePublisher,
            IStringManager stringManager)
        {
            _unitOfWork = unitOfWork;
            _reservationRequestRepository = reservationRequestRepository;
            _accommodationQueryHelper = accommodationQueryHelper;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _messagePublisher = messagePublisher;
            _stringManager = stringManager;
        }
        
        public async Task<ReservationRequest> Handle(CreateReservationRequest request, CancellationToken cancellationToken)
        {
            var accomodation = await _accommodationQueryHelper.ReadOrThrow(request.AccomodationId, cancellationToken);
            await Validate(request, cancellationToken);

            await _unitOfWork.Begin(cancellationToken);

            var reservationRequest = _mapper.Map<ReservationRequest>(request);

            var createdReservationRequest = await CreateRequest(reservationRequest, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            await PublishReservationRequestCreatedEvent(accomodation.HostId, createdReservationRequest, cancellationToken);

            return createdReservationRequest;
        }

        private async Task Validate(CreateReservationRequest reservationRequest, CancellationToken cancellationToken)
        {
            var datePeriod = _mapper.Map<DatePeriod>(reservationRequest.DatePeriod);
            if (await _reservationRepository.HasAnyByAccomodationAndDatePeriod(reservationRequest.AccomodationId, datePeriod, cancellationToken))
            {
                throw new BadLogicException(_stringManager.GetString("Request_Validation_AlreadyBooked"));
            }
        }

        private async Task<ReservationRequest> CreateRequest(ReservationRequest request, CancellationToken cancellationToken)
        {
            request.Status = ReservationRequestStatus.Waiting;

            return await _reservationRequestRepository.Create(request, cancellationToken);
        }

        private async Task PublishReservationRequestCreatedEvent(Guid hostId, ReservationRequest reservationRequest, CancellationToken cancellationToken)
        {
            var requestCreated = new ReservationRequestCreatedEvent() 
            { 
                ReservationRequestId = reservationRequest.Id,
                AccommodationId = reservationRequest.AccomodationId,
                HostId = hostId,
                GuestId = reservationRequest.GuestId,
            };

            await _messagePublisher.Send<ReservationRequestCreatedEvent, string>(requestCreated, cancellationToken);
        }
    }
}
