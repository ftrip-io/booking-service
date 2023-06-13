using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reservations.UseCases.CountActiveReservations
{
    public class CountActiveReservationsForGuestQueryPermissionProcessor : IRequestPreProcessor<CountActiveReservationsForGuestQuery>
    {
        private readonly IReservationsPermissionHelper _reservationsPermissionHelper;

        public CountActiveReservationsForGuestQueryPermissionProcessor(IReservationsPermissionHelper reservationsPermissionHelper)
        {
            _reservationsPermissionHelper = reservationsPermissionHelper;
        }

        public Task Process(CountActiveReservationsForGuestQuery request, CancellationToken cancellationToken)
        {
            _reservationsPermissionHelper.CanBeReadByCurrentGuest(request.GuestId);

            return Task.CompletedTask;
        }
    }
}