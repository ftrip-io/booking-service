using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.booking_service.Reviews.Domain
{
    public class AccomodationReviewsSummary : Entity<Guid>
    {
        public Guid AccomodationId { get; set; }

        public int ReviewsCount { get; set; }

        public AccomodationGradesSummary Grades { get; set; }

        public AccomodationReviewsSummary()
        {
            Grades = new AccomodationGradesSummary();
        }
    }

    public class AccomodationGradesSummary
    {
        public decimal Accomodation { get; set; }

        public decimal Location { get; set; }

        public decimal ValueForMoney { get; set; }

        public decimal Average { get => (Accomodation + Location + ValueForMoney) / 3.0M; }
    }
}