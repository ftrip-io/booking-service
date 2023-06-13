using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.booking_service.contracts.Reviews.Events
{
    public class AccomodationReviewDeletedEvent : Event<string>
    {
        public Guid AccomodationId { get; set; }

        public AccomodationReviewDeletedEvent()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}