﻿using ftrip.io.booking_service.contracts.ReservationRequests.Events;
using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.booking_service.ReservationRequests.UseCases.DeclineReservationRequests;
using ftrip.io.booking_service.ReservationRequests.UseCases.ReadReservationRequest;
using ftrip.io.framework.Contexts;
using ftrip.io.framework.Correlation;
using MassTransit;
using MediatR;
using Serilog;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.Consumers
{
    public class ReservationRequestAcceptedEventConsumer : IConsumer<ReservationRequestAcceptedEvent>
    {
        private readonly IReservationRequestRepository _reservationRequestRepository;
        private readonly CurrentUserContext _currentUserContext;
        private readonly CorrelationContext _correlationContext;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public ReservationRequestAcceptedEventConsumer(
            IReservationRequestRepository reservationRequestRepository,
            CurrentUserContext currentUserContext,
            CorrelationContext correlationContext,
            IMediator mediator,
            ILogger logger)
        {
            _reservationRequestRepository = reservationRequestRepository;
            _currentUserContext = currentUserContext;
            _correlationContext = correlationContext;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReservationRequestAcceptedEvent> context)
        {
            _currentUserContext.Id = "Consumer";
            _correlationContext.Id = context.CorrelationId.ToString();

            var requestAcceptedEvent = context.Message;

            var query = new ReadReservationRequestQuery()
            {
                AccommodationId = requestAcceptedEvent.AccomodationId,
                PeriodFrom = requestAcceptedEvent.From,
                PeriodTo = requestAcceptedEvent.To,
                Status = ReservationRequestStatus.Waiting,
            };

            var requestsToDecline = await _reservationRequestRepository.ReadByQuery(query, CancellationToken.None);

            var requestIdsToDecline = requestsToDecline
                .Where(request => request.Id != requestAcceptedEvent.RequestId)
                .Select(request => request.Id);
            foreach (var requestIdToDecline in requestIdsToDecline)
            {
                await _mediator.Send(new DeclineReservationRequest() { ReservationRequestId = requestIdToDecline });

                _logger.Information("Automatically declining Reservation Requests - RequestId[{RequestId}]", requestIdToDecline);
            }
        }
    }
}