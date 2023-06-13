using ftrip.io.booking_service.contracts.Reviews.Events;
using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.UpdateAccomodationReview
{
    public class UpdateAccomodationReviewRequestHandler : IRequestHandler<UpdateAccomodationReviewRequest, AccomodationReview>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccomodationReviewRepository _accomodationReviewRepository;
        private readonly IAccomodationReviewQueryHelper _accomodationReviewQueryHelper;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger _logger;

        public UpdateAccomodationReviewRequestHandler(
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

        public async Task<AccomodationReview> Handle(UpdateAccomodationReviewRequest request, CancellationToken cancellationToken)
        {
            var existingReview = await _accomodationReviewQueryHelper.ReadOrThrow(request.ReviewId, cancellationToken);

            existingReview.Grades.Accomodation = request.AccomodationGrade;
            existingReview.Grades.Location = request.LocationGrade;
            existingReview.Grades.ValueForMoney = request.ValueForMoneyGrade;
            existingReview.Recension.Text = request.RecensionText;

            await _unitOfWork.Begin(cancellationToken);

            var updatedReview = await Update(existingReview, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            await PublishAccomodationReviewUpdatedEvent(updatedReview, cancellationToken);

            return updatedReview;
        }

        private async Task<AccomodationReview> Update(AccomodationReview accomodationReview, CancellationToken cancellationToken)
        {
            var updatedReview = await _accomodationReviewRepository.Update(accomodationReview, cancellationToken);

            _logger.Information("Accommodation Review updated - ReviewId[{ReviewId}], Average[{Average}]", accomodationReview.Id, accomodationReview.Grades.Average);

            return updatedReview;
        }

        private async Task PublishAccomodationReviewUpdatedEvent(AccomodationReview review, CancellationToken cancellationToken)
        {
            var accomodationReviewUpdated = new AccomodationReviewUpdatedEvent()
            {
                AccomodationId = review.AccomodationId,
            };

            await _messagePublisher.Send<AccomodationReviewUpdatedEvent, string>(accomodationReviewUpdated, cancellationToken);
        }
    }
}