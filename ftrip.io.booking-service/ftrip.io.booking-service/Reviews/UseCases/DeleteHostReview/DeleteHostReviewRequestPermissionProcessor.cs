using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.DeleteHostReview
{
    public class DeleteHostReviewRequestPermissionProcessor : IRequestPreProcessor<DeleteHostReviewRequest>
    {
        private readonly IHostReviewPermissionsHelper _hostReviewPermissionsHelper;

        public DeleteHostReviewRequestPermissionProcessor(IHostReviewPermissionsHelper hostReviewPermissionsHelper)
        {
            _hostReviewPermissionsHelper = hostReviewPermissionsHelper;
        }

        public async Task Process(DeleteHostReviewRequest request, CancellationToken cancellationToken)
        {
            await _hostReviewPermissionsHelper.IsWrittenByCurrentGuest(request.ReviewId, cancellationToken);
        }
    }
}