using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.booking_service.contracts.Reviews.Events
{
    public class AccomodationReviewUpdatedEvent : Event<string>
    {
        public Guid AccomodationId { get; set; }

        public AccomodationReviewUpdatedEvent()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}