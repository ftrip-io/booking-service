using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.booking_service.contracts.Reviews.Events
{
    public class HostReviewedEvent : Event<string>
    {
        public Guid HostId { get; set; }

        public Guid GuestId { get; set; }

        public decimal AverageGrade { get; set; }

        public HostReviewedEvent()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}