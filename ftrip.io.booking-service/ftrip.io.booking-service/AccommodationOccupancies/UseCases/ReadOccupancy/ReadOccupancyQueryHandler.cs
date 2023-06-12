using ftrip.io.booking_service.AccommodationOccupancies.Domain;
using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.booking_service.ReservationRequests.UseCases.ReadReservationRequest;
using ftrip.io.booking_service.Reservations.Domain;
using ftrip.io.booking_service.Reservations.UseCases.ReadReservation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationOccupancies.UseCases.ReadOccupancy
{
    public class ReadOccupancyQueryHandler : IRequestHandler<ReadOccupancyQuery, IEnumerable<AccommodationOccupancy>>
    {
        private readonly IMediator _mediator;

        public ReadOccupancyQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IEnumerable<AccommodationOccupancy>> Handle(ReadOccupancyQuery request, CancellationToken cancellationToken)
        {
            DefaultValuesIfNull(request);

            var reservationRequests = await GetReservationRequests(request, cancellationToken);
            var reservations = await GetReservations(request, cancellationToken);

            return GenerateOccupancies(reservationRequests, reservations);
        }

        private void DefaultValuesIfNull(ReadOccupancyQuery request)
        {
            if (!request.PeriodFrom.HasValue)
            {
                request.PeriodFrom = DateTime.MinValue;
            }

            if (!request.PeriodTo.HasValue)
            {
                request.PeriodTo = DateTime.MaxValue;
            }
        }

        private async Task<IEnumerable<ReservationRequest>> GetReservationRequests(ReadOccupancyQuery request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new ReadReservationRequestQuery()
            {
                AccommodationId = request.AccommodationId,
                PeriodFrom = request.PeriodFrom,
                PeriodTo = request.PeriodTo,
                Status = ReservationRequestStatus.Waiting
            }, cancellationToken);
        }

        private async Task<IEnumerable<Reservation>> GetReservations(ReadOccupancyQuery request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new ReadReservationQuery()
            {
                AccommodationId = request.AccommodationId,
                PeriodFrom = request.PeriodFrom,
                PeriodTo = request.PeriodTo,
                IncludeCancelled = false
            }, cancellationToken);
        }

        private List<AccommodationOccupancy> GenerateOccupancies(IEnumerable<ReservationRequest> reservationRequests, IEnumerable<Reservation> reservations)
        {
            var occupancies = reservationRequests.Select(r => new AccommodationOccupancy()
            {
                AccomodationId = r.AccomodationId,
                DatePeriod = r.DatePeriod,
                OccupancyType = AccommodationOccupancyType.Request
            }).ToList();

            return occupancies.Concat(reservations.Select(r => new AccommodationOccupancy()
            {
                AccomodationId = r.AccomodationId,
                DatePeriod = r.DatePeriod,
                OccupancyType = AccommodationOccupancyType.Reservation
            })).ToList();
        }
    }
}
