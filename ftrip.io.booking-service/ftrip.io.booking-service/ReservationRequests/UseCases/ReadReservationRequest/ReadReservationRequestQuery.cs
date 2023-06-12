using Amazon.Runtime.Internal;
using ftrip.io.booking_service.ReservationRequests.Domain;
using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.ReadReservationRequest
{
    public class ReadReservationRequestQuery : IRequest<IEnumerable<ReservationRequest>>
    {
        public Guid? GuestId { get; set; }
        public Guid? AccommodationId { get; set; }
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public ReservationRequestStatus? Status { get; set; }
    }
}
