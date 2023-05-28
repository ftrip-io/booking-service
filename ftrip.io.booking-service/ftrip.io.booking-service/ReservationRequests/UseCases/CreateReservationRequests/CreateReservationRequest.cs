using ftrip.io.booking_service.Common.Domain;
using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.framework.Mapping;
using MediatR;
using System;

namespace ftrip.io.booking_service.ReservationRequests.UseCases.CreateReservationRequests
{
    [Mappable(Destination = typeof(ReservationRequest))]
    public class CreateReservationRequest : IRequest<ReservationRequest>
    {
        public Guid GuestId { get; set; }
        public Guid AccomodationId { get; set; }
        public CreateDatePeriodRequest DatePeriod { get; set; }
        public int GuestNumber { get; set; }
    }

    [Mappable(Destination = typeof(DatePeriod))]
    public class CreateDatePeriodRequest
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
