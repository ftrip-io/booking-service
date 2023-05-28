using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.booking_service.contracts.ReservationRequests.Events
{
    public class ReservationRequestDeclinedEvent : Event<string>
    {
        public Guid RequestId { get; set; }
        public Guid AccomodationId { get; set; }
        public Guid GuestId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public ReservationRequestDeclinedEvent()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
