using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.DeleteReservationRequests
{
    public class DeleteReservationRequestHandler : IRequestHandler<DeleteReservationRequest, ReservationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservationRequestRepository _reservationRequestRepository;
        private readonly IReservationRequestQueryHelper _reservationRequestQueryHelper;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public DeleteReservationRequestHandler(
            IUnitOfWork unitOfWork,
            IReservationRequestRepository reservationRequestRepository,
            IReservationRequestQueryHelper reservationRequestQueryHelper,
            IStringManager stringManager,
            ILogger logger)
        {
            _stringManager = stringManager;
            _reservationRequestRepository = reservationRequestRepository;
            _reservationRequestQueryHelper = reservationRequestQueryHelper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ReservationRequest> Handle(DeleteReservationRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var existingReservationRequest = await _reservationRequestQueryHelper.ReadOrThrow(request.ReservationRequestId, cancellationToken);
            Validate(existingReservationRequest);

            await DeleteRequest(existingReservationRequest, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            return existingReservationRequest;
        }

        private void Validate(ReservationRequest reservationRequest)
        {
            if (!reservationRequest.CanBeModified)
            {
                _logger.Error(
                    "Reservation Request cannot be accepted because state is different from Waiting - RequestId[{RequestId}], Status[{Status}]",
                    reservationRequest.Id, reservationRequest.Status
                );
                throw new BadLogicException(_stringManager.Format("Request_Validation_InvalidStatus", reservationRequest.Id, "deleted"));
            }
        }

        private async Task<ReservationRequest> DeleteRequest(ReservationRequest request, CancellationToken cancellationToken)
        {
            var deletedRequest = await _reservationRequestRepository.Delete(request.Id, cancellationToken);

            _logger.Information("Reservation Request deleted - RequestId[{RequestId}]", deletedRequest.Id);

            return deletedRequest;
        }
    }
}