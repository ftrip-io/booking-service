using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.UpdateHostReview
{
    public class UpdateHostReviewRequestPermissionProcessor : IRequestPreProcessor<UpdateHostReviewRequest>
    {
        private readonly IHostReviewPermissionsHelper _hostReviewPermissionsHelper;

        public UpdateHostReviewRequestPermissionProcessor(IHostReviewPermissionsHelper hostReviewPermissionsHelper)
        {
            _hostReviewPermissionsHelper = hostReviewPermissionsHelper;
        }

        public async Task Process(UpdateHostReviewRequest request, CancellationToken cancellationToken)
        {
            await _hostReviewPermissionsHelper.IsWrittenByCurrentGuest(request.ReviewId, cancellationToken);
        }
    }
}