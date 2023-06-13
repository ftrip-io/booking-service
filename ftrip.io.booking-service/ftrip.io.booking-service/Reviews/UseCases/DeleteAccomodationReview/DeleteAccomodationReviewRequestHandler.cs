using ftrip.io.booking_service.contracts.Reviews.Events;
using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.DeleteAccomodationReview
{
    public class DeleteAccomodationReviewRequestHandler : IRequestHandler<DeleteAccomodationReviewRequest, AccomodationReview>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccomodationReviewRepository _accomodationReviewRepository;
        private readonly IAccomodationReviewQueryHelper _accomodationReviewQueryHelper;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger _logger;

        public DeleteAccomodationReviewRequestHandler(
            IUnitOfWork unitOfWork,
            IAccomodationReviewRepository accomodationReviewRepository,
            IAccomodationReviewQueryHelper accomodationReviewQueryHelper,
            IMessagePublisher messagePublisher,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _accomodationReviewRepository = accomodationReviewRepository;
            _accomodationReviewQueryHelper = accomodationReviewQueryHelper;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<AccomodationReview> Handle(DeleteAccomodationReviewRequest request, CancellationToken cancellationToken)
        {
            var existingReview = await _accomodationReviewQueryHelper.ReadOrThrow(request.ReviewId, cancellationToken);

            await _unitOfWork.Begin(cancellationToken);

            var deletedReview = await Delete(existingReview.Id, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            await PublishAccomodationReviewDeletedEvent(deletedReview, cancellationToken);

            return deletedReview;
        }

        private async Task<AccomodationReview> Delete(Guid reviewId, CancellationToken cancellationToken)
        {
            var deletedReview = await _accomodationReviewRepository.Delete(reviewId, cancellationToken);

            _logger.Information("Accommodation Review delete - ReviewId[{ReviewId}]", reviewId);

            return deletedReview;
        }

        private async Task PublishAccomodationReviewDeletedEvent(AccomodationReview review, CancellationToken cancellationToken)
        {
            var accomodationReviewDeleted = new AccomodationReviewDeletedEvent()
            {
                AccomodationId = review.AccomodationId,
            };

            await _messagePublisher.Send<AccomodationReviewDeletedEvent, string>(accomodationReviewDeleted, cancellationToken);
        }
    }
}