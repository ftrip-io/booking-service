using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.UseCases
{
    public interface IReservationRequestQueryHelper
    {
        Task<ReservationRequest> ReadOrThrow(Guid requestId, CancellationToken cancellationToken);
    }

    public class ReservationRequestQueryHelper : IReservationRequestQueryHelper
    {
        private readonly IReservationRequestRepository _reservationRequestRepository;
        private readonly IStringManager _stringManager;

        public ReservationRequestQueryHelper(
            IReservationRequestRepository reservationRequestRepository,
            IStringManager stringManager)
        {
            _reservationRequestRepository = reservationRequestRepository;
            _stringManager = stringManager;
        }

        public async Task<ReservationRequest> ReadOrThrow(Guid requestId, CancellationToken cancellationToken)
        {
            var reservationRequest = await _reservationRequestRepository.Read(requestId, cancellationToken);
            if (reservationRequest == null)
            {
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", requestId));
            }

            return reservationRequest;
        }
    }
}
