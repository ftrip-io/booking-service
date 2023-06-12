using ftrip.io.booking_service.ReservationRequests.Domain;
using MediatR;
using System;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.DeleteReservationRequests
{
    public class DeleteReservationRequest : IRequest<ReservationRequest>
    {
        public Guid ReservationRequestId { get; set; }
    }
}
