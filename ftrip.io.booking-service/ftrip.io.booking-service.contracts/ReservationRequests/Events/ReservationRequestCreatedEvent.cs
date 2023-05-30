using ftrip.io.framework.Domain;
using System;


namespace ftrip.io.booking_service.contracts.ReservationRequests.Events
{
    public class ReservationRequestCreatedEvent : Event<string>
    {
        public Guid ReservationRequestId { get; set; }
        public Guid AccommodationId { get; set; }
        public Guid HostId { get; set; }
        public Guid GuestId { get; set; }

        public ReservationRequestCreatedEvent()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
