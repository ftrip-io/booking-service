using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationConfiguration.UseCases.ChangeAccommodationConfigurationRequests
{
    public class ChangeAccommodationConfigurationRequestHandler : IRequestHandler<ChangeAccommodationConfigurationRequest, Accommodation>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IAccommodationQueryHelper _accommodationQueryHelper;
        private readonly ILogger _logger;

        public ChangeAccommodationConfigurationRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IAccommodationQueryHelper accommodationQueryHelper,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _accommodationRepository = accommodationRepository;
            _accommodationQueryHelper = accommodationQueryHelper;
            _logger = logger;
        }

        public async Task<Accommodation> Handle(ChangeAccommodationConfigurationRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var existingAccommodation = await _accommodationQueryHelper.ReadOrThrow(request.AccommodationId, cancellationToken);

            await ChangeAccommodationConfiguration(existingAccommodation, request.IsManualAccept, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            return existingAccommodation;
        }

        public async Task<Accommodation> ChangeAccommodationConfiguration(Accommodation accommodation, bool isManualAccept, CancellationToken cancellationToken)
        {
            accommodation.IsManualAccept = isManualAccept;

            var updatedAccommodation = await _accommodationRepository.Update(accommodation, cancellationToken);

            _logger.Information(
                "Accommodation Configuration updated - AccommodationId[{AccommodationId}], ManualAccept[{ManualAccept}]",
                updatedAccommodation.AccommodationId, updatedAccommodation.IsManualAccept
            );

            return updatedAccommodation;
        }
    }
}