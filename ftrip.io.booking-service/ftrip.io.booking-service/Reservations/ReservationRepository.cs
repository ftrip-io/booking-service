using ftrip.io.booking_service.Common.Domain;
using ftrip.io.booking_service.Reservations.Domain;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reservations
{
    public interface IReservationRepository : IRepository<Reservation, Guid> 
    {
        Task<bool> HasAnyByAccomodationAndDatePeriod(Guid accomodationId, DatePeriod period, CancellationToken cancellationToken);
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
    }
}
