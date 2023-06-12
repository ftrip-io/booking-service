using ftrip.io.booking_service.Reservations.Domain;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reservations.UseCases.ReadReservation
{
    public class ReadReservationQueryHandler : IRequestHandler<ReadReservationQuery, IEnumerable<Reservation>>
    {
        private readonly IReservationRepository _reservationRepository;

        public ReadReservationQueryHandler(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<Reservation>> Handle(ReadReservationQuery request, CancellationToken cancellationToken)
        {
            return await _reservationRepository.ReadByQuery(request, cancellationToken);
        }
    }
}
