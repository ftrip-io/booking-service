using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.framework.Persistence.UtilityClasses;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.ReadAccomodationReviews
{
    public class ReadAccomodationReviewsQueryHandler : IRequestHandler<ReadAccomodationReviewsQuery, PageResult<AccomodationReview>>
    {
        private readonly IAccomodationReviewRepository _accomodationReviewRepository;

        public ReadAccomodationReviewsQueryHandler(IAccomodationReviewRepository accomodationReviewRepository)
        {
            _accomodationReviewRepository = accomodationReviewRepository;
        }

        public async Task<PageResult<AccomodationReview>> Handle(ReadAccomodationReviewsQuery query, CancellationToken cancellationToken)
        {
            return await _accomodationReviewRepository.ReadByQuery(query, cancellationToken);
        }
    }
}