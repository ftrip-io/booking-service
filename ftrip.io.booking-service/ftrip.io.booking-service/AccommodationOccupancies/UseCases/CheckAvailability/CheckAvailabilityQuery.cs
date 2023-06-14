using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.booking_service.AccommodationOccupancies.UseCases.CheckAvailability
{
    public class CheckAvailabilityQuery : IRequest<IEnumerable<Guid>>
    {
        public Guid[] AccommodationIds { get; set; }
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }

    }
}
