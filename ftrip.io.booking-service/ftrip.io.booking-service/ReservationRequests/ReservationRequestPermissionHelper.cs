using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.framework.Contexts;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests
{
    public interface IReservationRequestPermissionHelper
    {
        Task CanBeProcessedByCurrentHost(Guid requestId, CancellationToken cancellationToken);

        void CanBeRequestedByCurrentGuest(Guid guestId);

        Task IsRequestedByCurrentGuest(Guid requestId, CancellationToken cancellationToken);
    }

    public class ReservationRequestPermissionHelper : IReservationRequestPermissionHelper
    {
        private readonly IReservationRequestQueryHelper _reservationRequestQueryHelper;
        private readonly IAccommodationQueryHelper _accommodationQueryHelper;
        private readonly CurrentUserContext _currentUserContext;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public ReservationRequestPermissionHelper(
            IReservationRequestQueryHelper reservationRequestQueryHelper,
            IAccommodationQueryHelper accommodationQueryHelper,
            CurrentUserContext currentUserContext,
            IStringManager stringManager,
            ILogger logger)
        {
            _reservationRequestQueryHelper = reservationRequestQueryHelper;
            _accommodationQueryHelper = accommodationQueryHelper;
            _currentUserContext = currentUserContext;
            _stringManager = stringManager;
            _logger = logger;
        }

        public async Task CanBeProcessedByCurrentHost(Guid requestId, CancellationToken cancellationToken)
        {
            if (_currentUserContext.Id == "Consumer")
                return;

            var request = await _reservationRequestQueryHelper.ReadOrThrow(requestId, cancellationToken);
            var accommodation = await _accommodationQueryHelper.ReadOrThrow(request.AccomodationId, cancellationToken);
            var isProcessedByHost = accommodation.HostId.ToString() == _currentUserContext.Id;
            if (!isProcessedByHost)
            {
                _logger.Error("Error while trying to execute action for other host - HostId[{HostId}], ExecutingAsId[{ExecutingAsId}]", _currentUserContext.Id, accommodation.HostId);
                throw new ForbiddenException(_stringManager.Format("Requests_CannotExecuteForThatRequest", requestId));
            }
        }

        public void CanBeRequestedByCurrentGuest(Guid guestId)
        {
            var doingForHimself = guestId.ToString() == _currentUserContext.Id;
            if (!doingForHimself)
            {
                _logger.Error("Error while trying to execute action for other guest - GuestId[{GuestId}], ExecutingAsId[{ExecutingAsId}]", _currentUserContext.Id, guestId);
                throw new ForbiddenException(_stringManager.Format("Requests_CannotExecuteForThatGuest", guestId));
            }
        }

        public async Task IsRequestedByCurrentGuest(Guid requestId, CancellationToken cancellationToken)
        {
            var request = await _reservationRequestQueryHelper.ReadOrThrow(requestId, cancellationToken);
            var isRequestGuest = request.GuestId.ToString() == _currentUserContext.Id;
            if (!isRequestGuest)
            {
                _logger.Error("Error while trying to execute action for other guest - GuestId[{GuestId}], ExecutingAsId[{ExecutingAsId}]", _currentUserContext.Id, request.GuestId);
                throw new ForbiddenException(_stringManager.Format("Requests_CannotExecuteForThatRequest", requestId));
            }
        }
    }
}