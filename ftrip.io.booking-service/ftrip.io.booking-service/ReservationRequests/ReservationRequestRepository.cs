using ftrip.io.booking_service.ReservationRequests.Domain;
using ftrip.io.booking_service.ReservationRequests.UseCases.ReadReservationRequest;
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
        Task<IEnumerable<ReservationRequest>> ReadByQuery(ReadReservationRequestQuery query, CancellationToken cancellationToken);

    }

    public class ReservationRequestRepository : Repository<ReservationRequest, Guid>, IReservationRequestRepository
    {
        public ReservationRequestRepository(DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ReservationRequest>> ReadByQuery(ReadReservationRequestQuery query, CancellationToken cancellationToken)
        {
            return await _entities
                 .Where(r => !query.Status.HasValue || r.Status == query.Status)
                 .Where(r => !query.GuestId.HasValue || r.GuestId == query.GuestId)
                 .Where(r => !query.AccommodationId.HasValue || r.AccomodationId == query.AccommodationId)
                 .Where(r => ((r.DatePeriod.DateFrom <= query.PeriodTo && r.DatePeriod.DateTo >= query.PeriodFrom) ||
                              (r.DatePeriod.DateFrom >= query.PeriodFrom && r.DatePeriod.DateFrom <= query.PeriodTo)))
                 .ToListAsync(cancellationToken);
        }
    }
}
