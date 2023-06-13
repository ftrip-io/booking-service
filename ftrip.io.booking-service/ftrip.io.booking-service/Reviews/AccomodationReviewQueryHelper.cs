using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews
{
    public interface IAccomodationReviewQueryHelper
    {
        Task<AccomodationReview> ReadOrThrow(Guid reviewId, CancellationToken cancellationToken);
    }

    public class AccomodationReviewQueryHelper : IAccomodationReviewQueryHelper
    {
        private readonly IAccomodationReviewRepository _accomodationReviewRepository;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public AccomodationReviewQueryHelper(
            IAccomodationReviewRepository accomodationReviewRepository,
            IStringManager stringManager,
            ILogger logger)
        {
            _accomodationReviewRepository = accomodationReviewRepository;
            _stringManager = stringManager;
            _logger = logger;
        }

        public async Task<AccomodationReview> ReadOrThrow(Guid reviewId, CancellationToken cancellationToken)
        {
            var accomodationReview = await _accomodationReviewRepository.Read(reviewId, cancellationToken);
            if (accomodationReview == null)
            {
                _logger.Error("Accomodation Review not found - ReviewId[{ReviewId}]", reviewId);
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", reviewId));
            }

            return accomodationReview;
        }
    }
}