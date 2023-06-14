using ftrip.io.booking_service.Common.Domain;
using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.booking_service.ReservationRequests.Domain
{
    public class ReservationRequest : Entity<Guid>
    {
        public Guid GuestId { get; set; }
        public Guid AccomodationId { get; set; }
        public DatePeriod DatePeriod { get; set; }
        public int GuestNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public ReservationRequestStatus Status { get; set; }

        public bool CanBeModified { get => Status == ReservationRequestStatus.Waiting; }
    }
}
