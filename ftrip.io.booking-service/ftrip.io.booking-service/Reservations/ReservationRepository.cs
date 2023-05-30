using ftrip.io.booking_service.Common.Domain;
using ftrip.io.booking_service.Reservations.Domain;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ftrip.io.booking_service.Reservations.UseCases.ReadReservation;

namespace ftrip.io.booking_service.Reservations
{
    public interface IReservationRepository : IRepository<Reservation, Guid> 
    {
        Task<bool> HasAnyByAccomodationAndDatePeriod(Guid accomodationId, DatePeriod period, CancellationToken cancellationToken);
        Task<IEnumerable<Reservation>> ReadByQuery(ReadReservationQuery query, CancellationToken cancellationToken);
    }
    public class ReservationRepository : Repository<Reservation, Guid>, IReservationRepository
    {
        public ReservationRepository(DbContext context) : base(context)
        {
        }
        public async Task<bool> HasAnyByAccomodationAndDatePeriod(Guid accomodationId, DatePeriod period, CancellationToken cancellationToken)
        {
            return await _entities.Where(r => r.AccomodationId == accomodationId && !r.IsCancelled &&
                                        ((r.DatePeriod.DateFrom <= period.DateTo && r.DatePeriod.DateTo >= period.DateFrom) ||
                                        (r.DatePeriod.DateFrom >= period.DateFrom && r.DatePeriod.DateFrom <= period.DateTo)))
                                  .AnyAsync(cancellationToken);
        }

        public async Task<IEnumerable<Reservation>> ReadByQuery(ReadReservationQuery query, CancellationToken cancellationToken)
        {
            return await _entities
                .Where(r => (!query.IncludeCancelled.GetValueOrDefault() && !r.IsCancelled) || query.IncludeCancelled.Value)
                .Where(r => !query.GuestId.HasValue || r.GuestId == query.GuestId)
                .Where(r => !query.AccommodationId.HasValue || r.AccomodationId == query.AccommodationId)
                .Where(r => !query.PeriodFrom.HasValue || r.DatePeriod.DateFrom >= query.PeriodFrom)
                .Where(r => !query.PeriodTo.HasValue || r.DatePeriod.DateTo <= query.PeriodTo)
                .ToListAsync(cancellationToken);
        }
    }
}
