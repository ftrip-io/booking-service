using ftrip.io.booking_service.Reservations.Domain;
using MediatR;
using System;

namespace ftrip.io.booking_service.Reservations.UseCases.CancelReservation
{
    public class CancelReservationRequest : IRequest<Reservation>
    {
        public Guid ReservationId { get; set; }
    }
}
