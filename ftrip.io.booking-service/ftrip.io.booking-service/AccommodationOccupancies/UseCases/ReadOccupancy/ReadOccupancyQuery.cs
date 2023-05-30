using ftrip.io.booking_service.AccommodationOccupancies.Domain;
using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.booking_service.AccommodationOccupancies.UseCases.ReadOccupancy
{
    public class ReadOccupancyQuery : IRequest<IEnumerable<AccommodationOccupancy>>
    {
        public Guid? GuestId { get; set; }
        public Guid? AccommodationId { get; set; }
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
    }
}
