using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews.UseCases.GetPossibleHostsForReview
{
    public class GetPossibleHostsForReviewQueryPermissionProcessor : IRequestPreProcessor<GetPossibleHostsForReviewQuery>
    {
        private readonly IHostReviewPermissionsHelper _hostReviewPermissionsHelper;

        public GetPossibleHostsForReviewQueryPermissionProcessor(IHostReviewPermissionsHelper hostReviewPermissionsHelper)
        {
            _hostReviewPermissionsHelper = hostReviewPermissionsHelper;
        }

        public Task Process(GetPossibleHostsForReviewQuery request, CancellationToken cancellationToken)
        {
            _hostReviewPermissionsHelper.CanBeWrittenByCurrentGuest(request.GuestId);

            return Task.CompletedTask;
        }
    }
}