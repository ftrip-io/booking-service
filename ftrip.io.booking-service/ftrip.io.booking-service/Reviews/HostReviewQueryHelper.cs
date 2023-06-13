using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews
{
    public interface IHostReviewQueryHelper
    {
        Task<HostReview> ReadOrThrow(Guid reviewId, CancellationToken cancellationToken);
    }

    public class HostReviewQueryHelper : IHostReviewQueryHelper
    {
        private readonly IHostReviewRepository _hostReviewRepository;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public HostReviewQueryHelper(
            IHostReviewRepository hostReviewRepository,
            IStringManager stringManager,
            ILogger logger)
        {
            _hostReviewRepository = hostReviewRepository;
            _stringManager = stringManager;
            _logger = logger;
        }

        public async Task<HostReview> ReadOrThrow(Guid reviewId, CancellationToken cancellationToken)
        {
            var hostReview = await _hostReviewRepository.Read(reviewId, cancellationToken);
            if (hostReview == null)
            {
                _logger.Error("Host Review not found - ReviewId[{ReviewId}]", reviewId);
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", reviewId));
            }

            return hostReview;
        }
    }
}