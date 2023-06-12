using ftrip.io.booking_service.ReservationRequests.Domain;
using MediatR;
using System;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.AcceptReservationRequests
{
    public class AcceptReservationRequest : IRequest<ReservationRequest>
    {
        public Guid ReservationRequestId { get; set; }
    }
}
