using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.booking_service.Reviews.Domain
{
    public class HostReview : Entity<Guid>
    {
        public Guid GuestId { get; set; }
        public Guid HostId { get; set; }

        public HostGrades Grades { get; set; }
        public HostRecension Recension { get; set; }
    }

    public class HostGrades
    {
        public int Communication { get; set; }

        public int Overall { get; set; }

        public decimal Average { get => (Communication + Overall) / 2.0M; }
    }

    public class HostRecension
    {
        public string Text { get; set; }
    }
}