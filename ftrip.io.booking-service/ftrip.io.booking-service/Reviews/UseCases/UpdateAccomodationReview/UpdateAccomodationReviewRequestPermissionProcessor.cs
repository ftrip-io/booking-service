using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.UpdateAccomodationReview
{
    public class UpdateAccomodationReviewRequestPermissionProcessor : IRequestPreProcessor<UpdateAccomodationReviewRequest>
    {
        private readonly IAccommodationReviewPermissionsHelper _accommodationReviewPermissionsHelper;

        public UpdateAccomodationReviewRequestPermissionProcessor(IAccommodationReviewPermissionsHelper accommodationReviewPermissionsHelper)
        {
            _accommodationReviewPermissionsHelper = accommodationReviewPermissionsHelper;
        }

        public async Task Process(UpdateAccomodationReviewRequest request, CancellationToken cancellationToken)
        {
            await _accommodationReviewPermissionsHelper.IsWrittenByCurrentGuest(request.ReviewId, cancellationToken);
        }
    }
}