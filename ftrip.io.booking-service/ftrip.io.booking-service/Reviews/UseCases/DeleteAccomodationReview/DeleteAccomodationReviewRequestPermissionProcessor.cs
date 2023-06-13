using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.DeleteAccomodationReview
{
    public class DeleteAccomodationReviewRequestPermissionProcessor : IRequestPreProcessor<DeleteAccomodationReviewRequest>
    {
        private readonly IAccommodationReviewPermissionsHelper _accommodationReviewPermissionsHelper;

        public DeleteAccomodationReviewRequestPermissionProcessor(IAccommodationReviewPermissionsHelper accommodationReviewPermissionsHelper)
        {
            _accommodationReviewPermissionsHelper = accommodationReviewPermissionsHelper;
        }

        public async Task Process(DeleteAccomodationReviewRequest request, CancellationToken cancellationToken)
        {
            await _accommodationReviewPermissionsHelper.IsWrittenByCurrentGuest(request.ReviewId, cancellationToken);
        }
    }
}