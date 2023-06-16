using ftrip.io.booking_service.ReservationRequests.Domain;
using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.DeleteReservationRequestsByGuest
{
    public class DeleteReservationRequestsByGuestRequest : IRequest<IEnumerable<ReservationRequest>>
    {
        public Guid GuestId { get; set; }
    }
}