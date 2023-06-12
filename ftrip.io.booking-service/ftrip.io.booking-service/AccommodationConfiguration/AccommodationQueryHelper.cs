using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationConfiguration
{
    public interface IAccommodationQueryHelper
    {
        Task<Accommodation> ReadOrThrow(Guid accommodationId, CancellationToken cancellationToken);
    }

    public class AccommodationQueryHelper : IAccommodationQueryHelper
    {
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public AccommodationQueryHelper(
            IAccommodationRepository accommodationRepository,
            IStringManager stringManager,
            ILogger logger)
        {
            _accommodationRepository = accommodationRepository;
            _stringManager = stringManager;
            _logger = logger;
        }

        public async Task<Accommodation> ReadOrThrow(Guid accommodationId, CancellationToken cancellationToken)
        {
            var accommodation = await _accommodationRepository.ReadByAccommodationId(accommodationId, cancellationToken);
            if (accommodation == null)
            {
                _logger.Error("Accommodation Configuration not found - AccommodationId[{AccommodationId}]", accommodationId);
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", accommodationId));
            }

            return accommodation;
        }
    }
}