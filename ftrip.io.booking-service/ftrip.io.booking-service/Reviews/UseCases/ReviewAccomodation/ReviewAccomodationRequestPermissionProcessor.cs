using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.ReviewAccomodation
{
    public class ReviewAccomodationRequestPermissionProcessor : IRequestPreProcessor<ReviewAccomodationRequest>
    {
        private readonly IAccommodationReviewPermissionsHelper _accommodationReviewPermissionsHelper;

        public ReviewAccomodationRequestPermissionProcessor(IAccommodationReviewPermissionsHelper accommodationReviewPermissionsHelper)
        {
            _accommodationReviewPermissionsHelper = accommodationReviewPermissionsHelper;
        }

        public Task Process(ReviewAccomodationRequest request, CancellationToken cancellationToken)
        {
            _accommodationReviewPermissionsHelper.CanBeWrittenByCurrentGuest(request.GuestId);

            return Task.CompletedTask;
        }
    }
}