using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.booking_service.contracts.Reviews.Events
{
    public class AccomodationReviewedEvent : Event<string>
    {
        public Guid AccomodationId { get; set; }

        public Guid HostId { get; set; }

        public Guid GuestId { get; set; }

        public decimal AverageGrade { get; set; }

        public AccomodationReviewedEvent()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}