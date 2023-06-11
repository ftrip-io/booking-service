using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests
{
    public interface IReservationRequestQueryHelper
    {
        Task<ReservationRequest> ReadOrThrow(Guid requestId, CancellationToken cancellationToken);
    }

    public class ReservationRequestQueryHelper : IReservationRequestQueryHelper
    {
        private readonly IReservationRequestRepository _reservationRequestRepository;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public ReservationRequestQueryHelper(
            IReservationRequestRepository reservationRequestRepository,
            IStringManager stringManager,
            ILogger logger)
        {
            _reservationRequestRepository = reservationRequestRepository;
            _stringManager = stringManager;
            _logger = logger;
        }

        public async Task<ReservationRequest> ReadOrThrow(Guid requestId, CancellationToken cancellationToken)
        {
            var reservationRequest = await _reservationRequestRepository.Read(requestId, cancellationToken);
            if (reservationRequest == null)
            {
                _logger.Error("Reservation Request not found - RequestId[{RequestId}]", requestId);
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", requestId));
            }

            return reservationRequest;
        }
    }
}