using ftrip.io.booking_service.contracts.ReservationRequests.Events;
using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.DeclineReservationRequests
{
    public class DeclineReservationRequestHandler : IRequestHandler<DeclineReservationRequest, ReservationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservationRequestRepository _reservationRequestRepository;
        private readonly IReservationRequestQueryHelper _reservationRequestQueryHelper;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public DeclineReservationRequestHandler(
            IUnitOfWork unitOfWork,
            IReservationRequestRepository reservationRequestRepository,
            IReservationRequestQueryHelper reservationRequestQueryHelper,
            IMessagePublisher messagePublisher,
            IStringManager stringManager,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _reservationRequestQueryHelper = reservationRequestQueryHelper;
            _reservationRequestRepository = reservationRequestRepository;
            _messagePublisher = messagePublisher;
            _stringManager = stringManager;
            _logger = logger;
        }

        public async Task<ReservationRequest> Handle(DeclineReservationRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var existingReservationRequest = await _reservationRequestQueryHelper.ReadOrThrow(request.ReservationRequestId, cancellationToken);
            Validate(existingReservationRequest);

            await DeclineRequest(existingReservationRequest, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            await PublishReservationRequestDeclinedEvent(existingReservationRequest, cancellationToken);

            return existingReservationRequest;
        }

        private void Validate(ReservationRequest reservationRequest)
        {
            if (!reservationRequest.CanBeModified)
            {
                _logger.Error(
                    "Reservation Request cannot be declined because state is different from Waiting - RequestId[{RequestId}], Status[{Status}]",
                    reservationRequest.Id, reservationRequest.Status
                );
                throw new BadLogicException(_stringManager.Format("Request_Validation_InvalidStatus", reservationRequest.Id, "declined"));
            }
        }

        private async Task<ReservationRequest> DeclineRequest(ReservationRequest request, CancellationToken cancellationToken)
        {
            request.Status = ReservationRequestStatus.Declined;

            var declinedRequest = await _reservationRequestRepository.Update(request, cancellationToken);

            _logger.Information("Reservation Request declined - RequestId[{RequestId}]", declinedRequest.Id);

            return declinedRequest;
        }

        private async Task PublishReservationRequestDeclinedEvent(ReservationRequest request, CancellationToken cancellationToken)
        {
            var requestDeclined = new ReservationRequestDeclinedEvent()
            {
                RequestId = request.Id,
                AccomodationId = request.AccomodationId,
                GuestId = request.GuestId,
                From = request.DatePeriod.DateFrom,
                To = request.DatePeriod.DateTo,
            };

            await _messagePublisher.Send<ReservationRequestDeclinedEvent, string>(requestDeclined, cancellationToken);
        }
    }
}