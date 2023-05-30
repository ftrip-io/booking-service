using ftrip.io.booking_service.ReservationRequests.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.ReadReservationRequest
{
    public class ReadReservationRequestQueryHandler : IRequestHandler<ReadReservationRequestQuery, IEnumerable<ReservationRequest>>
    {
        private readonly IReservationRequestRepository _reservationRequestRepository;

        public ReadReservationRequestQueryHandler(IReservationRequestRepository reservationRequestRepository)
        {
            _reservationRequestRepository = reservationRequestRepository;
        }

        public async Task<IEnumerable<ReservationRequest>> Handle(ReadReservationRequestQuery request, CancellationToken cancellationToken)
        {
            if (!request.PeriodFrom.HasValue) 
            {
                request.PeriodFrom = DateTime.MinValue; 
            }

            if (!request.PeriodTo.HasValue) 
            {
                request.PeriodTo = DateTime.MaxValue;
            }

            return await _reservationRequestRepository.ReadByQuery(request, cancellationToken);
        }
    }
}
