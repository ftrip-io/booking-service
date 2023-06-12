using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationConfiguration
{
    public interface IAccommodationRepository : IRepository<Accommodation, Guid>
    {
        Task<Accommodation> ReadByAccommodationId(Guid accommodationId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Accommodation>> ReadByAccommodationIds(IEnumerable<Guid> accommodationIds, CancellationToken cancellationToken = default);

        Task<IEnumerable<Accommodation>> ReadByHostId(Guid hostId, CancellationToken cancellationToken = default);
    }

    public class AccommodationRepository : Repository<Accommodation, Guid>, IAccommodationRepository
    {
        public AccommodationRepository(DbContext context) : base(context)
        {
        }

        public async Task<Accommodation> ReadByAccommodationId(Guid accommodationId, CancellationToken cancellationToken = default)
        {
            return await _entities.FirstOrDefaultAsync(a => a.AccommodationId == accommodationId, cancellationToken);
        }

        public async Task<IEnumerable<Accommodation>> ReadByAccommodationIds(IEnumerable<Guid> accommodationIds, CancellationToken cancellationToken = default)
        {
            return await _entities.Where(a => accommodationIds.Contains(a.AccommodationId)).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Accommodation>> ReadByHostId(Guid hostId, CancellationToken cancellationToken = default)
        {
            return await _entities.Where(a => a.HostId == hostId).ToListAsync(cancellationToken);
        }
    }
}