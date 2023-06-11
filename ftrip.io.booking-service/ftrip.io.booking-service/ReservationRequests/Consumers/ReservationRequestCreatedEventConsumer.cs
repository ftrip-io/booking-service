﻿using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.booking_service.contracts.ReservationRequests.Events;
using ftrip.io.booking_service.ReservationRequests.UseCases.AcceptReservationRequests;
using MassTransit;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.Consumers
{
    public class ReservationRequestCreatedEventConsumer : IConsumer<ReservationRequestCreatedEvent>
    {
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public ReservationRequestCreatedEventConsumer(
            IAccommodationRepository accommodationRepository,
            IMediator mediator,
            ILogger logger)
        {
            _accommodationRepository = accommodationRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReservationRequestCreatedEvent> context)
        {
            var requestCreatedEvent = context.Message;

            var accommodation = await _accommodationRepository.ReadByAccommodationId(requestCreatedEvent.AccommodationId, CancellationToken.None);
            var shouldAutomaticallyAccept = !accommodation?.IsManualAccept ?? false;
            if (!shouldAutomaticallyAccept)
            {
                return;
            }

            _logger.Information("Automatically accepting Reservation Request - RequestId[{RequestId}]", requestCreatedEvent.ReservationRequestId);

            await _mediator.Send(new AcceptReservationRequest() { ReservationRequestId = requestCreatedEvent.ReservationRequestId }, CancellationToken.None);
        }
    }
}