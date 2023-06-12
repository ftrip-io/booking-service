using MediatR.Pipeline;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationConfiguration.UseCases.ChangeAccommodationConfigurationRequests
{
    public class ChangeAccommodationConfigurationRequestPermissionProcessor : IRequestPreProcessor<ChangeAccommodationConfigurationRequest>
    {
        private readonly IAccommodationPermissionHelper _accommodationPermissionHelper;

        public ChangeAccommodationConfigurationRequestPermissionProcessor(IAccommodationPermissionHelper accommodationPermissionHelper)
        {
            _accommodationPermissionHelper = accommodationPermissionHelper;
        }

        public async Task Process(ChangeAccommodationConfigurationRequest request, CancellationToken cancellationToken)
        {
            await _accommodationPermissionHelper.IsHostedByCurrentUser(request.AccommodationId, cancellationToken);
        }
    }
}