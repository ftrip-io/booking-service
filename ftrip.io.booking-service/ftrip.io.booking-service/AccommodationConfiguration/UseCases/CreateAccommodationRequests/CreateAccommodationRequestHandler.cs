using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationConfiguration.UseCases.CreateAccommodationRequests
{
    public class CreateAccommodationRequestHandler : IRequestHandler<CreateAccommodationRequest, Accommodation>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly ILogger _logger;

        public CreateAccommodationRequestHandler(
            IUnitOfWork unitOfWork,
            IAccommodationRepository accommodationRepository,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _accommodationRepository = accommodationRepository;
            _logger = logger;
        }

        public async Task<Accommodation> Handle(CreateAccommodationRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var createdAccommodation = await CreateAccommodation(request, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            return createdAccommodation;
        }

        private async Task<Accommodation> CreateAccommodation(CreateAccommodationRequest request, CancellationToken cancellationToken)
        {
            var createAccommodation = new Accommodation()
            {
                AccommodationId = request.AccommodationId,
                HostId = request.HostId,
                IsManualAccept = true
            };

            var createdAccommodation = await _accommodationRepository.Create(createAccommodation, cancellationToken);

            _logger.Information(
                "Accommodation Configuration created - AccommodationId[{AccommodationId}], HostId[{HostId}], ManualAccept[{ManualAccept}]",
                createAccommodation.AccommodationId, createAccommodation.HostId, createAccommodation.IsManualAccept
            );

            return createdAccommodation;
        }
    }
}