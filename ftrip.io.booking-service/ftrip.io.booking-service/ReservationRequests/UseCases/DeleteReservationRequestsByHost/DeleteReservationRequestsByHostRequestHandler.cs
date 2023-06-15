using ftrip.io.booking_service.AccommodationConfiguration;
using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.booking_service.ReservationRequests.UseCases.ReadReservationRequest;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.DeleteReservationRequestsByHost
{
    public class DeleteReservationRequestsByHostRequestHandler : IRequestHandler<DeleteReservationRequestsByHostRequest, IEnumerable<ReservationRequest>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IReservationRequestRepository _reservationRequestRepository;

        public DeleteReservationRequestsByHostRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IReservationRequestRepository reservationRequestRepository)
        {
            _unitOfWork = unitOfWork;
            _accommodationRepository = accommodationRepository;
            _reservationRequestRepository = reservationRequestRepository;
        }

        public async Task<IEnumerable<ReservationRequest>> Handle(DeleteReservationRequestsByHostRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var accommodations = await _accommodationRepository.ReadByHostId(request.HostId);
            var requests = new List<ReservationRequest>();
            foreach (var accommodationId in accommodations.Select(a => a.AccommodationId))
            {
                var query = new ReadReservationRequestQuery()
                {
                    AccommodationId = accommodationId,
                    Status = ReservationRequestStatus.Waiting,
                    PeriodFrom = DateTime.MinValue,
                    PeriodTo = DateTime.MaxValue
                };
                requests.AddRange(await _reservationRequestRepository.ReadByQuery(query, cancellationToken));
            }

            var deletedRequests = await _reservationRequestRepository.DeleteRange(requests, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            return deletedRequests;
        }
    }
}