using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.DeclineReservationRequests
{
    public class DeclineReservationRequestPermissionProcessor : IRequestPreProcessor<DeclineReservationRequest>
    {
        private readonly IReservationRequestPermissionHelper _reservationRequestPermissionHelper;

        public DeclineReservationRequestPermissionProcessor(IReservationRequestPermissionHelper reservationRequestPermissionHelper)
        {
            _reservationRequestPermissionHelper = reservationRequestPermissionHelper;
        }

        public async Task Process(DeclineReservationRequest request, CancellationToken cancellationToken)
        {
            await _reservationRequestPermissionHelper.CanBeProcessedByCurrentHost(request.ReservationRequestId, cancellationToken);
        }
    }
}