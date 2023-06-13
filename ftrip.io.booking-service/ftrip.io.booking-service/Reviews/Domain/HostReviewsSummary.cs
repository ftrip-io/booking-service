using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.booking_service.Reviews.Domain
{
    public class HostReviewsSummary : Entity<Guid>
    {
        public Guid HostId { get; set; }

        public int ReviewsCount { get; set; }

        public HostGradesSummary Grades { get; set; }
    }

    public class HostGradesSummary
    {
        public decimal Communication { get; set; }

        public decimal Overall { get; set; }

        public decimal Average { get => (Communication + Overall) / 2.0M; }
    }
}