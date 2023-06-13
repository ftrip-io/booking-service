using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reservations.UseCases.CountActiveReservations
{
    public class CountActiveReservationsForGuestQueryHandler : IRequestHandler<CountActiveReservationsForGuestQuery, int>
    {
        private readonly IReservationRepository _reservationRepository;

        public CountActiveReservationsForGuestQueryHandler(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<int> Handle(CountActiveReservationsForGuestQuery request, CancellationToken cancellationToken)
        {
            return await _reservationRepository.CountActiveForGuest(request.GuestId, cancellationToken);
        }
    }
}