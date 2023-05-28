using ftrip.io.booking_service.Common.Domain;
using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests
{
    public interface IReservationRequestRepository : IRepository<ReservationRequest, Guid>
    {
        Task<IEnumerable<ReservationRequest>> ReadByAccomodationAndDatePeriod(Guid accomodationId, DatePeriod period, CancellationToken cancellationToken);
    }

    public class ReservationRequestRepository : Repository<ReservationRequest, Guid>, IReservationRequestRepository
    {
        public ReservationRequestRepository(DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ReservationRequest>> ReadByAccomodationAndDatePeriod(Guid accomodationId, DatePeriod period, CancellationToken cancellationToken)
        {
            return await _entities.Where(r => r.AccomodationId == accomodationId &&
                                        ((r.DatePeriod.DateFrom <= period.DateTo && r.DatePeriod.DateTo >= period.DateFrom) ||
                                        (r.DatePeriod.DateFrom >= period.DateFrom && r.DatePeriod.DateFrom <= period.DateTo)))
                                  .ToListAsync(cancellationToken);
        }
    }
}
