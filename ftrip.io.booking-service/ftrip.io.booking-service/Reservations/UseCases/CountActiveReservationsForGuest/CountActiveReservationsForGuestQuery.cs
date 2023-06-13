using MediatR;
using System;

namespace ftrip.io.booking_service.Reservations.UseCases.CountActiveReservations
{
    public class CountActiveReservationsForGuestQuery : IRequest<int>
    {
        public Guid GuestId { get; set; }
    }
}