using ftrip.io.framework.Contexts;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reservations
{
    public interface IReservationsPermissionHelper
    {
        Task IsReservedByCurrentGuest(Guid reservationId, CancellationToken cancellationToken);
    }

    public class ReservationsPermissionHelper : IReservationsPermissionHelper
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly CurrentUserContext _currentUserContext;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public ReservationsPermissionHelper(
            IReservationRepository reservationRepository,
            CurrentUserContext currentUserContext,
            IStringManager stringManager,
            ILogger logger)
        {
            _reservationRepository = reservationRepository;
            _currentUserContext = currentUserContext;
            _stringManager = stringManager;
            _logger = logger;
        }

        public async Task IsReservedByCurrentGuest(Guid reservationId, CancellationToken cancellationToken)
        {
            var reservation = await _reservationRepository.Read(reservationId, cancellationToken);
            var isReservationGuest = reservation?.GuestId.ToString() == _currentUserContext.Id;
            if (!isReservationGuest)
            {
                _logger.Error("Error while trying to execute action for other guest - GuestId[{GuestId}], ExecutingAsId[{ExecutingAsId}]", _currentUserContext.Id, reservation.GuestId);
                throw new ForbiddenException(_stringManager.Format("Reservations_CannotExecuteForThatReservation", reservationId));
            }
        }
    }
}