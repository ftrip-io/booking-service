using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationConfiguration.UseCases.ChangeAccommodationConfigurationRequests
{
    public class ChangeAccommodationConfigurationRequestHandler : IRequestHandler<ChangeAccommodationConfigurationRequest, Accommodation>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IStringManager _stringManager;

        public ChangeAccommodationConfigurationRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            IStringManager stringManager)
        {
            _unitOfWork = unitOfWork;
            _accommodationRepository = accommodationRepository;
            _stringManager = stringManager;
        }
        public async Task<Accommodation> Handle(ChangeAccommodationConfigurationRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var existingAccommodation = await ReadOrThrow(request.AccommodationId, cancellationToken);

            await ChangeAccommodationConfiguration(existingAccommodation, request.IsManualAccept, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            return existingAccommodation;
        }

        public async Task<Accommodation> ReadOrThrow(Guid accommodationId, CancellationToken cancellationToken)
        {
            var existingAccommodation = await _accommodationRepository.ReadByAccommodationId(accommodationId, cancellationToken);
            if (existingAccommodation == null)
            {
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", existingAccommodation.AccommodationId));
            }

            return existingAccommodation;
        }

        public async Task<Accommodation> ChangeAccommodationConfiguration(Accommodation accommodation, bool isManualAccpet, CancellationToken cancellationToken)
        {
            accommodation.IsManualAccept = isManualAccpet;
                
            return await _accommodationRepository.Update(accommodation, cancellationToken);
        }
    }
}
