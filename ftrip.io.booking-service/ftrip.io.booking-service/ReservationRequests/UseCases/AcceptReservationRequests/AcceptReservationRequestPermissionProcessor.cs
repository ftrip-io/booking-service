using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.AcceptReservationRequests
{
    public class AcceptReservationRequestPermissionProcessor : IRequestPreProcessor<AcceptReservationRequest>
    {
        private readonly IReservationRequestPermissionHelper _reservationRequestPermissionHelper;

        public AcceptReservationRequestPermissionProcessor(IReservationRequestPermissionHelper reservationRequestPermissionHelper)
        {
            _reservationRequestPermissionHelper = reservationRequestPermissionHelper;
        }

        public async Task Process(AcceptReservationRequest request, CancellationToken cancellationToken)
        {
            await _reservationRequestPermissionHelper.CanBeProcessedByCurrentHost(request.ReservationRequestId, cancellationToken);
        }
    }
}