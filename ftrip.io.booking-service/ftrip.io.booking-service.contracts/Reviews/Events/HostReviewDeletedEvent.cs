using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.booking_service.contracts.Reviews.Events
{
    public class HostReviewDeletedEvent : Event<string>
    {
        public Guid HostId { get; set; }

        public HostReviewDeletedEvent()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}