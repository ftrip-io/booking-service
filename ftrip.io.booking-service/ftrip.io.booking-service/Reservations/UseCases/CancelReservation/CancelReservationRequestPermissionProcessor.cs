using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reservations.UseCases.CancelReservation
{
    public class CancelReservationRequestPermissionProcessor : IRequestPreProcessor<CancelReservationRequest>
    {
        private readonly IReservationsPermissionHelper _reservationsPermissionHelper;

        public CancelReservationRequestPermissionProcessor(IReservationsPermissionHelper reservationsPermissionHelper)
        {
            _reservationsPermissionHelper = reservationsPermissionHelper;
        }

        public async Task Process(CancelReservationRequest request, CancellationToken cancellationToken)
        {
            await _reservationsPermissionHelper.IsReservedByCurrentGuest(request.ReservationId, cancellationToken);
        }
    }
}