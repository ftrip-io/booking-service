using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationConfiguration
{
    public interface IAccommodationRepository: IRepository<Accommodation, Guid>
    {
        Task<Accommodation> ReadByAccommodationId(Guid accommodationId, CancellationToken cancellationToken = default);
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
    }
}
