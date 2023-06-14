using ftrip.io.booking_service.Reservations;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationOccupancies.UseCases.CheckAvailability
{
    public class CheckAvailabilityQueryHandler : IRequestHandler<CheckAvailabilityQuery, IEnumerable<Guid>>
    {
        private readonly IReservationRepository _reservationRepository;
        public CheckAvailabilityQueryHandler(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<Guid>> Handle(CheckAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var notAvailableAccommodationId = await _reservationRepository.ReadByAccommodationsAndDatePeriod(request, cancellationToken);

            return request.AccommodationIds.Except(notAvailableAccommodationId).ToList();

        }
    }
}
