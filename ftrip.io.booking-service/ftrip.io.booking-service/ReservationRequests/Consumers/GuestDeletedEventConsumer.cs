using ftrip.io.booking_service.ReservationRequests.UseCases.DeleteReservationRequestsByGuest;
using ftrip.io.user_service.contracts.Users.Events;
using MassTransit;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.Consumers
{
    public class GuestDeletedEventConsumer : IConsumer<UserDeletedEvent>
    {
        private readonly IMediator _mediator;

        public GuestDeletedEventConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<UserDeletedEvent> context)
        {
            var userDeleted = context.Message;
            if (userDeleted.UserType != "Guest")
            {
                return;
            }

            await _mediator.Send(new DeleteReservationRequestsByGuestRequest() { GuestId = Guid.Parse(userDeleted.UserId) }, CancellationToken.None);
        }
    }
}