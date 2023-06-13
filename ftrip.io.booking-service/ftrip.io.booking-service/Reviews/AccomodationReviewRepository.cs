using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.booking_service.Reviews.UseCases.ReadAccomodationReviews;
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
    public interface IAccomodationReviewRepository : IRepository<AccomodationReview, Guid>
    {
        Task<bool> IsReviewByGuestForAccomodation(Guid guestId, Guid accomodationId, CancellationToken cancellationToken);

        Task<PageResult<AccomodationReview>> ReadByQuery(ReadAccomodationReviewsQuery query, CancellationToken cancellationToken);

        Task<IEnumerable<AccomodationReview>> ReadByGuestId(Guid guestId, CancellationToken cancellationToken);
    }

    public class AccomodationReviewRepository : Repository<AccomodationReview, Guid>, IAccomodationReviewRepository
    {
        public AccomodationReviewRepository(DbContext context) :
            base(context)
        {
        }

        public Task<bool> IsReviewByGuestForAccomodation(Guid guestId, Guid accomodationId, CancellationToken cancellationToken)
        {
            return _entities.AnyAsync(r => r.GuestId == guestId && r.AccomodationId == accomodationId, cancellationToken);
        }

        public Task<PageResult<AccomodationReview>> ReadByQuery(ReadAccomodationReviewsQuery query, CancellationToken cancellationToken)
        {
            var filters = query.Filters;
            var page = query.Page;

            return _entities
                .AsNoTracking()
                .Where(r => !filters.AccomodationId.HasValue || r.AccomodationId == filters.AccomodationId)
                .Where(r => !filters.GuestId.HasValue || r.GuestId == filters.GuestId)
                .Where(r => !filters.GradeFrom.HasValue || (r.Grades.Accomodation + r.Grades.Location + r.Grades.ValueForMoney) / 3.0M >= filters.GradeFrom)
                .Where(r => !filters.GradeTo.HasValue || (r.Grades.Accomodation + r.Grades.Location + r.Grades.ValueForMoney) / 3.0M <= filters.GradeTo)
                .Where(r => string.IsNullOrEmpty(filters.RecentionText) || r.Recension.Text.ToLower().Contains(filters.RecentionText.ToLower()))
                .OrderByDescending(r => r.CreatedAt)
                .ToPageResult(page);
        }

        public async Task<IEnumerable<AccomodationReview>> ReadByGuestId(Guid guestId, CancellationToken cancellationToken)
        {
            return await _entities.Where(r => r.GuestId == guestId).ToListAsync(cancellationToken);
        }
    }
}