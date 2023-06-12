using ftrip.io.framework.Contexts;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationConfiguration
{
    public interface IAccommodationPermissionHelper
    {
        Task IsHostedByCurrentUser(Guid accommodationId, CancellationToken cancellationToken);
    }

    public class AccommodationPermissionHelper : IAccommodationPermissionHelper
    {
        private readonly IAccommodationQueryHelper _accommodationQueryHelper;
        private readonly CurrentUserContext _currentUserContext;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public AccommodationPermissionHelper(
            IAccommodationQueryHelper accommodationQueryHelper,
            CurrentUserContext currentUserContext,
            IStringManager stringManager,
            ILogger logger)
        {
            _accommodationQueryHelper = accommodationQueryHelper;
            _currentUserContext = currentUserContext;
            _stringManager = stringManager;
            _logger = logger;
        }

        public async Task IsHostedByCurrentUser(Guid accommodationId, CancellationToken cancellationToken)
        {
            var accommodation = await _accommodationQueryHelper.ReadOrThrow(accommodationId, cancellationToken);
            var isExecutedByHost = accommodation.HostId.ToString() == _currentUserContext.Id;
            if (!isExecutedByHost)
            {
                _logger.Error("Error while trying to execute action for other host - HostId[{HostId}], ExecutingAsId[{ExecutingAsId}]", _currentUserContext.Id, accommodation.HostId);
                throw new ForbiddenException(_stringManager.Format("Accommodations_NotExecutedByHost", accommodationId));
            }
        }
    }
}