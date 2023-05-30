using ftrip.io.booking_service.Reservations.Domain;
using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.booking_service.Reservations.UseCases.ReadReservation
{
    public class ReadReservationQuery : IRequest<IEnumerable<Reservation>>
    {
        public Guid? GuestId { get; set; }
        public Guid? AccommodationId { get; set; }
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public bool? IncludeCancelled { get; set; }
    }
}
