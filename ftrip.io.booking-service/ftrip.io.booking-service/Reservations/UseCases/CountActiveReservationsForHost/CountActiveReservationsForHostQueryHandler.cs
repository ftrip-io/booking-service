using ftrip.io.booking_service.AccommodationConfiguration;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reservations.UseCases.CountActiveReservationsForHost
{
    public class CountActiveReservationsForHostQueryHandler : IRequestHandler<CountActiveReservationsForHostQuery, int>
    {
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IReservationRepository _reservationRepository;

        public CountActiveReservationsForHostQueryHandler(
            IAccommodationRepository accommodationRepository,
            IReservationRepository reservationRepository)
        {
            _accommodationRepository = accommodationRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<int> Handle(CountActiveReservationsForHostQuery request, CancellationToken cancellationToken)
        {
            var accommodation = await _accommodationRepository.ReadByHostId(request.HostId, cancellationToken);
            var accommodationIds = accommodation.Select(accommodation => accommodation.AccommodationId);

            return await _reservationRepository.CountActiveByAccommodations(accommodationIds, cancellationToken);
        }
    }
}