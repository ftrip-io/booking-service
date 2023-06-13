using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.booking_service.Reviews.UseCases.ReadHostReviews;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Extensions;
using ftrip.io.framework.Persistence.Sql.Repository;
using ftrip.io.framework.Persistence.UtilityClasses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews
{
    public interface IHostReviewRepository : IRepository<HostReview, Guid>
    {
        Task<bool> IsHostReviewByGuest(Guid guestId, Guid hostId, CancellationToken cancellationToken);

        Task<PageResult<HostReview>> ReadByQuery(ReadHostReviewsQuery query, CancellationToken cancellationToken);

        Task<IEnumerable<HostReview>> ReadByGuestId(Guid guestId, CancellationToken cancellationToken);
    }

    public class HostReviewRepository : Repository<HostReview, Guid>, IHostReviewRepository
    {
        public HostReviewRepository(DbContext context) :
            base(context)
        {
        }

        public Task<bool> IsHostReviewByGuest(Guid guestId, Guid hostId, CancellationToken cancellationToken)
        {
            return _entities.AnyAsync(r => r.GuestId == guestId && r.HostId == hostId, cancellationToken);
        }

        public Task<PageResult<HostReview>> ReadByQuery(ReadHostReviewsQuery query, CancellationToken cancellationToken)
        {
            var filters = query.Filters;
            var page = query.Page;

            return _entities
                .AsNoTracking()
                .Where(r => !filters.HostId.HasValue || r.HostId == filters.HostId)
                .Where(r => !filters.GuestId.HasValue || r.GuestId == filters.GuestId)
                .Where(r => !filters.GradeFrom.HasValue || (r.Grades.Communication + r.Grades.Overall) / 2.0M >= filters.GradeFrom)
                .Where(r => !filters.GradeTo.HasValue || (r.Grades.Communication + r.Grades.Overall) / 2.0M <= filters.GradeTo)
                .Where(r => string.IsNullOrEmpty(filters.RecentionText) || r.Recension.Text.ToLower().Contains(filters.RecentionText.ToLower()))
                .OrderByDescending(r => r.CreatedAt)
                .ToPageResult(page);
        }

        public async Task<IEnumerable<HostReview>> ReadByGuestId(Guid guestId, CancellationToken cancellationToken)
        {
            return await _entities.Where(r => r.GuestId == guestId).ToListAsync(cancellationToken);
        }
    }
}