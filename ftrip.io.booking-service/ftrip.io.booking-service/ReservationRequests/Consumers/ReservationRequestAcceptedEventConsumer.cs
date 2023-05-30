using ftrip.io.booking_service.Common.Domain;
using ftrip.io.booking_service.contracts.ReservationRequests.Events;
using ftrip.io.booking_service.ReservationRequests.UseCases.DeclineReservationRequests;
using ftrip.io.booking_service.ReservationRequests.UseCases.ReadReservationRequest;
using MassTransit;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ftrip.io.booking_service.ReservationRequests.Consumers
{
    public class ReservationRequestAcceptedEventConsumer : IConsumer<ReservationRequestAcceptedEvent>
    {
        private readonly IReservationRequestRepository _reservationRequestRepository;
        private readonly IMediator _mediator;

        public ReservationRequestAcceptedEventConsumer(
            IReservationRequestRepository reservationRequestRepository,
            IMediator mediator)
        {
            _reservationRequestRepository = reservationRequestRepository;
            _mediator = mediator;
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
            var requestsToDeclineHandlings = requestsToDecline
                .Where(request => request.Id != requestAcceptedEvent.RequestId)
                .Select(request => _mediator.Send(new DeclineReservationRequest() {  ReservationRequestId = request.Id }))
                .ToList();

            await Task.WhenAll(requestsToDeclineHandlings);


        }
    }
}
