using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.booking_service.Reviews.Domain
{
    public class AccomodationReview : Entity<Guid>
    {
        public Guid GuestId { get; set; }
        public Guid AccomodationId { get; set; }

        public AccomodationGrades Grades { get; set; }
        public AccomodationRecension Recension { get; set; }

        public AccomodationReview()
        {
            Grades = new AccomodationGrades();
            Recension = new AccomodationRecension();
        }
    }

    public class AccomodationGrades
    {
        public int Accomodation { get; set; }

        public int Location { get; set; }

        public int ValueForMoney { get; set; }

        public decimal Average { get => (Accomodation + Location + ValueForMoney) / 3.0M; }
    }

    public class AccomodationRecension
    {
        public string Text { get; set; }
    }
}