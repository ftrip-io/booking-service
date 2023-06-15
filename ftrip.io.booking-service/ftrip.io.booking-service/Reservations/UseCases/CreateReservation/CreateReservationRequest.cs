using ftrip.io.booking_service.Common.Domain;
using ftrip.io.booking_service.Reservations.Domain;
using ftrip.io.framework.Mapping;
using MediatR;
using System;

namespace ftrip.io.booking_service.Reservations.UseCases.CreateReservation
{
    [Mappable(Destination = typeof(Reservation))]
    public class CreateReservationRequest : IRequest<Reservation>
    {
        public Guid GuestId { get; set; }
        public Guid AccomodationId { get; set; }
        public CreateDatePeriodRequest DatePeriod { get; set; }
        public int GuestNumber { get; set; }
        public decimal TotalPrice { get; set; }
    }

    [Mappable(Destination = typeof(DatePeriod))]
    public class CreateDatePeriodRequest
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
