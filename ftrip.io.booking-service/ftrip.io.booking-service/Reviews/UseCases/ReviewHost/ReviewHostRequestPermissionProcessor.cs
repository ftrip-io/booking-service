using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.ReviewHost
{
    public class ReviewHostRequestPermissionProcessor : IRequestPreProcessor<ReviewHostRequest>
    {
        private readonly IHostReviewPermissionsHelper _hostReviewPermissionsHelper;

        public ReviewHostRequestPermissionProcessor(IHostReviewPermissionsHelper hostReviewPermissionsHelper)
        {
            _hostReviewPermissionsHelper = hostReviewPermissionsHelper;
        }

        public Task Process(ReviewHostRequest request, CancellationToken cancellationToken)
        {
            _hostReviewPermissionsHelper.CanBeWrittenByCurrentGuest(request.GuestId);

            return Task.CompletedTask;
        }
    }
}