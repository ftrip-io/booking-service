using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.DeleteReservationRequests
{
    public class DeleteReservationRequestPermissionProcessor : IRequestPreProcessor<DeleteReservationRequest>
    {
        private readonly IReservationRequestPermissionHelper _reservationRequestPermissionHelper;

        public DeleteReservationRequestPermissionProcessor(IReservationRequestPermissionHelper reservationRequestPermissionHelper)
        {
            _reservationRequestPermissionHelper = reservationRequestPermissionHelper;
        }

        public async Task Process(DeleteReservationRequest request, CancellationToken cancellationToken)
        {
            await _reservationRequestPermissionHelper.IsRequestedByCurrentGuest(request.ReservationRequestId, cancellationToken);
        }
    }
}