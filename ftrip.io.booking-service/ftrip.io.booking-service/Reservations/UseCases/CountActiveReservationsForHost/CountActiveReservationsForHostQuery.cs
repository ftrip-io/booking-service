using MediatR;
using System;

namespace ftrip.io.booking_service.Reservations.UseCases.CountActiveReservationsForHost
{
    public class CountActiveReservationsForHostQuery : IRequest<int>
    {
        public Guid HostId { get; set; }
    }
}