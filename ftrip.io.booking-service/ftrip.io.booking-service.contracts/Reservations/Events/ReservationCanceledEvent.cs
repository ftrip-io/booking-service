using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.booking_service.contracts.Reservations.Events
{
    public class ReservationCanceledEvent : Event<string>
    {
        public Guid ReservationId { get; set; }
        public Guid AccomodationId { get; set; }
        public Guid HostId { get; set; }
        public Guid GuestId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public ReservationCanceledEvent()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}