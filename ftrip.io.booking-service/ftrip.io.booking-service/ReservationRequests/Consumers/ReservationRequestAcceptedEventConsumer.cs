using ftrip.io.booking_service.contracts.ReservationRequests.Events;
using ftrip.io.booking_service.ReservationRequests.UseCases.DeclineReservationRequests;
using ftrip.io.booking_service.ReservationRequests.UseCases.ReadReservationRequest;
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
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public ReservationRequestAcceptedEventConsumer(
            IReservationRequestRepository reservationRequestRepository,
            IMediator mediator,
            ILogger logger)
        {
            _reservationRequestRepository = reservationRequestRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReservationRequestAcceptedEvent> context)
        {
            var requestAcceptedEvent = context.Message;

            var query = new ReadReservationRequestQuery()
            {
                AccommodationId = requestAcceptedEvent.AccomodationId,
                PeriodFrom = requestAcceptedEvent.From,
                PeriodTo = requestAcceptedEvent.To,
                GuestId = null
            };

            var requestsToDecline = await _reservationRequestRepository.ReadByQuery(query, CancellationToken.None);
            var requestIdsToDecline = requestsToDecline
                .Where(request => request.Id != requestAcceptedEvent.RequestId)
                .Select(request => request.Id);
            var requestsToDeclineHandlings = requestsToDecline
                .Select(request => _mediator.Send(new DeclineReservationRequest() { ReservationRequestId = request.Id }))
                .ToList();

            _logger.Information("Automatically declining Reservation Requests - RequestIds[{RequestIds}]", requestIdsToDecline);

            await Task.WhenAll(requestsToDeclineHandlings);
        }
    }
}