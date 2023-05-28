using ftrip.io.booking_service.Common.Domain;
using ftrip.io.booking_service.contracts.ReservationRequests.Events;
using ftrip.io.booking_service.ReservationRequests.UseCases.DeclineReservationRequests;
using MassTransit;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            var datePeriod = new DatePeriod()
            {
                DateFrom = requestAcceptedEvent.From,
                DateTo = requestAcceptedEvent.To
            };

            var requestsToDecline = await _reservationRequestRepository.ReadByAccomodationAndDatePeriod(requestAcceptedEvent.AccomodationId, datePeriod, CancellationToken.None);
            var requestsToDeclineHandlings = requestsToDecline
                .Where(request => request.Id != requestAcceptedEvent.RequestId)
                .Select(request => _mediator.Send(new DeclineReservationRequest() {  ReservationRequestId = request.Id }))
                .ToList();

            await Task.WhenAll(requestsToDeclineHandlings);


        }
    }
}
