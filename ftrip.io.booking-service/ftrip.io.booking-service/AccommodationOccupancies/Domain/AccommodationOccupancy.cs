using ftrip.io.booking_service.Common.Domain;
using System;

namespace ftrip.io.booking_service.AccommodationOccupancies.Domain
{
    public class AccommodationOccupancy
    {
        public Guid AccomodationId { get; set; }
        public DatePeriod DatePeriod { get; set; }
        public AccommodationOccupancyType OccupancyType { get; set; }
    }
}
