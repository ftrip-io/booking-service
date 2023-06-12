using ftrip.io.booking_service.Common.Domain;
using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.booking_service.Reservations.Domain
{
    public class Reservation : Entity<Guid>
    {
        public Guid GuestId { get; set; }
        public Guid AccomodationId { get; set; }
        public DatePeriod DatePeriod { get; set; }
        public int GuestNumber { get; set; }
        public bool IsCancelled { get; set; }
    }
}
