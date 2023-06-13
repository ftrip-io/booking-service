using ftrip.io.booking_service.contracts.Reviews.Events;
using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.UpdateHostReview
{
    public class UpdateHostReviewRequestHandler : IRequestHandler<UpdateHostReviewRequest, HostReview>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostReviewRepository _hostReviewRepository;
        private readonly IHostReviewQueryHelper _hostReviewQueryHelper;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger _logger;

        public UpdateHostReviewRequestHandler(
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

        public async Task<HostReview> Handle(UpdateHostReviewRequest request, CancellationToken cancellationToken)
        {
            var existingReview = await _hostReviewQueryHelper.ReadOrThrow(request.ReviewId, cancellationToken);

            existingReview.Grades.Communication = request.CommunicationGrade;
            existingReview.Grades.Overall = request.OverallGrade;
            existingReview.Recension.Text = request.RecensionText;

            await _unitOfWork.Begin(cancellationToken);

            var updatedReview = await Update(existingReview, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            await PublishHostReviewUpdatedEvent(updatedReview, cancellationToken);

            return updatedReview;
        }

        private async Task<HostReview> Update(HostReview hostReview, CancellationToken cancellationToken)
        {
            var updatedReview = await _hostReviewRepository.Update(hostReview, cancellationToken);

            _logger.Information("Host Review updated - ReviewId[{ReviewId}], Average[{Average}]", hostReview.Id, hostReview.Grades.Average);

            return updatedReview;
        }

        private async Task PublishHostReviewUpdatedEvent(HostReview review, CancellationToken cancellationToken)
        {
            var hostReviewUpdated = new HostReviewUpdatedEvent()
            {
                HostId = review.HostId,
            };

            await _messagePublisher.Send<HostReviewUpdatedEvent, string>(hostReviewUpdated, cancellationToken);
        }
    }
}