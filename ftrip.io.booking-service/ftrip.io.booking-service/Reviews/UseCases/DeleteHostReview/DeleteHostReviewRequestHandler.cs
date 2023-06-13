using ftrip.io.booking_service.contracts.Reviews.Events;
using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.DeleteHostReview
{
    public class DeleteHostReviewRequestHandler : IRequestHandler<DeleteHostReviewRequest, HostReview>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostReviewRepository _hostReviewRepository;
        private readonly IHostReviewQueryHelper _hostReviewQueryHelper;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger _logger;

        public DeleteHostReviewRequestHandler(
            IUnitOfWork unitOfWork,
            IHostReviewRepository hostReviewRepository,
            IHostReviewQueryHelper hostReviewQueryHelper,
            IMessagePublisher messagePublisher,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _hostReviewRepository = hostReviewRepository;
            _hostReviewQueryHelper = hostReviewQueryHelper;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<HostReview> Handle(DeleteHostReviewRequest request, CancellationToken cancellationToken)
        {
            var existingReview = await _hostReviewQueryHelper.ReadOrThrow(request.ReviewId, cancellationToken);

            await _unitOfWork.Begin(cancellationToken);

            var deletedReview = await Delete(existingReview.Id, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            await PublishHostReviewDeletedEvent(deletedReview, cancellationToken);

            return deletedReview;
        }

        private async Task<HostReview> Delete(Guid reviewId, CancellationToken cancellationToken)
        {
            var deletedReview = await _hostReviewRepository.Delete(reviewId, cancellationToken);

            _logger.Information("Host Review delete - ReviewId[{ReviewId}]", reviewId);

            return deletedReview;
        }

        private async Task PublishHostReviewDeletedEvent(HostReview review, CancellationToken cancellationToken)
        {
            var hostReviewDeleted = new HostReviewDeletedEvent()
            {
                HostId = review.HostId,
            };

            await _messagePublisher.Send<HostReviewDeletedEvent, string>(hostReviewDeleted, cancellationToken);
        }
    }
}