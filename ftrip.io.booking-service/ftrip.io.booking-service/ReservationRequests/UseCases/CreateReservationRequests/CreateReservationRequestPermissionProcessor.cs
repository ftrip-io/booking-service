using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.CreateReservationRequests
{
    public class CreateReservationRequestPermissionProcessor : IRequestPreProcessor<CreateReservationRequest>
    {
        private readonly IReservationRequestPermissionHelper _reservationRequestPermissionHelper;

        public CreateReservationRequestPermissionProcessor(IReservationRequestPermissionHelper reservationRequestPermissionHelper)
        {
            _reservationRequestPermissionHelper = reservationRequestPermissionHelper;
        }

        public Task Process(CreateReservationRequest request, CancellationToken cancellationToken)
        {
            _reservationRequestPermissionHelper.CanBeRequestedByCurrentGuest(request.GuestId);

            return Task.CompletedTask;
        }
    }
}