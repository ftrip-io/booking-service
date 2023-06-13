using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.GetPossibleAccommodationsForReview
{
    public class GetPossibleAccommodationsForReviewQueryPermissionProcessor : IRequestPreProcessor<GetPossibleAccommodationsForReviewQuery>
    {
        private readonly IAccommodationReviewPermissionsHelper _accommodationReviewPermissionsHelper;

        public GetPossibleAccommodationsForReviewQueryPermissionProcessor(IAccommodationReviewPermissionsHelper accommodationReviewPermissionsHelper)
        {
            _accommodationReviewPermissionsHelper = accommodationReviewPermissionsHelper;
        }

        public Task Process(GetPossibleAccommodationsForReviewQuery request, CancellationToken cancellationToken)
        {
            _accommodationReviewPermissionsHelper.CanBeWrittenByCurrentGuest(request.GuestId);

            return Task.CompletedTask;
        }
    }
}