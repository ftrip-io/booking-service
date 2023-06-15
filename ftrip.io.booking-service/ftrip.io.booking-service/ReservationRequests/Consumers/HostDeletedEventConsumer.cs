using ftrip.io.booking_service.ReservationRequests.UseCases.DeleteReservationRequestsByHost;
using ftrip.ip.user_service.contracts.Users.Events;
using MassTransit;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.Consumers
{
    public class HostDeletedEventConsumer : IConsumer<UserDeletedEvent>
    {
        private readonly IMediator _mediator;

        public HostDeletedEventConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<UserDeletedEvent> context)
        {
            var userDeleted = context.Message;
            if (userDeleted.UserType != "Host")
            {
                return;
            }

            await _mediator.Send(new DeleteReservationRequestsByHostRequest() { HostId = Guid.Parse(userDeleted.UserId) }, CancellationToken.None);
        }
    }
}