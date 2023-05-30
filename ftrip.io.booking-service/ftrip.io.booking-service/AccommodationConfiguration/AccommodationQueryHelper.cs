using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
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

        public AccommodationQueryHelper(
            IAccommodationRepository accommodationRepository,
            IStringManager stringManager)
        {
            _accommodationRepository = accommodationRepository;
            _stringManager = stringManager;
        }

        public async Task<Accommodation> ReadOrThrow(Guid accommodationId, CancellationToken cancellationToken)
        {
            var accommodation = await _accommodationRepository.Read(accommodationId, cancellationToken);
            if (accommodation == null)
            {
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", accommodationId));
            }

            return accommodation;
        }
    }
}
