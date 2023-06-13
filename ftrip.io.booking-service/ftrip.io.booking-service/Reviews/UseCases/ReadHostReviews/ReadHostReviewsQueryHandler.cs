using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.framework.Persistence.UtilityClasses;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.ReadHostReviews
{
    public class ReadHostReviewsQueryHandler : IRequestHandler<ReadHostReviewsQuery, PageResult<HostReview>>
    {
        private readonly IHostReviewRepository _hostReviewRepository;

        public ReadHostReviewsQueryHandler(IHostReviewRepository hostReviewRepository)
        {
            _hostReviewRepository = hostReviewRepository;
        }

        public async Task<PageResult<HostReview>> Handle(ReadHostReviewsQuery query, CancellationToken cancellationToken)
        {
            return await _hostReviewRepository.ReadByQuery(query, cancellationToken);
        }
    }
}