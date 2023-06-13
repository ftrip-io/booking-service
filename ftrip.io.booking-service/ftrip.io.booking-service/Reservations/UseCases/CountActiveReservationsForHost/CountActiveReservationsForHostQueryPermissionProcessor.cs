using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reservations.UseCases.CountActiveReservationsForHost
{
    public class CountActiveReservationsForHostQueryPermissionProcessor : IRequestPreProcessor<CountActiveReservationsForHostQuery>
    {
        private readonly IReservationsPermissionHelper _reservationsPermissionHelper;

        public CountActiveReservationsForHostQueryPermissionProcessor(IReservationsPermissionHelper reservationsPermissionHelper)
        {
            _reservationsPermissionHelper = reservationsPermissionHelper;
        }

        public Task Process(CountActiveReservationsForHostQuery request, CancellationToken cancellationToken)
        {
            _reservationsPermissionHelper.CanBeReadByCurrentGuest(request.HostId);

            return Task.CompletedTask;
        }
    }
}