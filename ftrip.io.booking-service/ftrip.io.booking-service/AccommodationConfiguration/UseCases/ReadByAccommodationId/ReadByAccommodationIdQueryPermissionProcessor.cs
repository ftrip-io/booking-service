using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationConfiguration.UseCases.ReadByAccommodationId
{
    public class ReadByAccommodationIdQueryPermissionProcessor : IRequestPreProcessor<ReadByAccommodationIdQuery>
    {
        private readonly IAccommodationPermissionHelper _accommodationPermissionHelper;

        public ReadByAccommodationIdQueryPermissionProcessor(IAccommodationPermissionHelper accommodationPermissionHelper)
        {
            _accommodationPermissionHelper = accommodationPermissionHelper;
        }

        public async Task Process(ReadByAccommodationIdQuery request, CancellationToken cancellationToken)
        {
            await _accommodationPermissionHelper.IsHostedByCurrentUser(request.AccommodationId, cancellationToken);
        }
    }
}