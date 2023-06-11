using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.framework.Contexts;
using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.ReadReservationRequest
{
    public class ReadReservationRequestQueryPermissionProcessor : IRequestPreProcessor<ReadReservationRequestQuery>
    {
        private readonly IReservationRequestPermissionHelper _reservationRequestPermissionHelper;
        private readonly IAccommodationPermissionHelper _accommodationPermissionHelper;
        private readonly CurrentUserContext _currentUserContext;

        public ReadReservationRequestQueryPermissionProcessor(
            IReservationRequestPermissionHelper reservationRequestPermissionHelper,
            IAccommodationPermissionHelper accommodationPermissionHelper,
            CurrentUserContext currentUserContext)
        {
            _reservationRequestPermissionHelper = reservationRequestPermissionHelper;
            _accommodationPermissionHelper = accommodationPermissionHelper;
            _currentUserContext = currentUserContext;
        }

        public async Task Process(ReadReservationRequestQuery request, CancellationToken cancellationToken)
        {
            if (_currentUserContext.Role == "Guest")
            {
                _reservationRequestPermissionHelper.CanBeRequestedByCurrentGuest(request.GuestId.GetValueOrDefault());
            }
            else
            {
                await _accommodationPermissionHelper.IsHostedByCurrentUser(request.AccommodationId.GetValueOrDefault(), cancellationToken);
            }
        }
    }
}