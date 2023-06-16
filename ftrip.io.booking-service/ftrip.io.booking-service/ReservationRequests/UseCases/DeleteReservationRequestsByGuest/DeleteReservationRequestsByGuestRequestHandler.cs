using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.booking_service.ReservationRequests.UseCases.ReadReservationRequest;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.DeleteReservationRequestsByGuest
{
    public class DeleteReservationRequestsByGuestRequestHandler : IRequestHandler<DeleteReservationRequestsByGuestRequest, IEnumerable<ReservationRequest>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservationRequestRepository _reservationRequestRepository;

        public DeleteReservationRequestsByGuestRequestHandler(
            IUnitOfWork unitOfWork,
            IReservationRequestRepository reservationRequestRepository)
        {
            _unitOfWork = unitOfWork;
            _reservationRequestRepository = reservationRequestRepository;
        }

        public async Task<IEnumerable<ReservationRequest>> Handle(DeleteReservationRequestsByGuestRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var query = new ReadReservationRequestQuery()
            {
                GuestId = request.GuestId,
                Status = ReservationRequestStatus.Waiting,
                PeriodFrom = DateTime.MinValue,
                PeriodTo = DateTime.MaxValue
            };
            var requests = await _reservationRequestRepository.ReadByQuery(query, cancellationToken);

            var deletedRequests = await _reservationRequestRepository.DeleteRange(requests, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            return deletedRequests;
        }
    }
}