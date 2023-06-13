using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.booking_service.contracts.Reviews.Events
{
    public class HostReviewUpdatedEvent : Event<string>
    {
        public Guid HostId { get; set; }

        public HostReviewUpdatedEvent()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}