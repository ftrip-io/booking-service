using ftrip.io.booking_service.ReservationRequests.Domain;
using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.DeleteReservationRequestsByHost
{
    public class DeleteReservationRequestsByHostRequest : IRequest<IEnumerable<ReservationRequest>>
    {
        public Guid HostId { get; set; }
    }
}