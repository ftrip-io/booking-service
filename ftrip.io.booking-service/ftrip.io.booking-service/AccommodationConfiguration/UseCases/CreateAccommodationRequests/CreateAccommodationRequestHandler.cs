using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using ftrip.io.framework.Persistence.Contracts;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationConfiguration.UseCases.CreateAccommodationRequests
{
    public class CreateAccommodationRequestHandler : IRequestHandler<CreateAccommodationRequest, Accommodation>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccommodationRepository _accommodationRepository;

        public CreateAccommodationRequestHandler(
            IUnitOfWork unitOfWork, 
            IAccommodationRepository accommodationRepository)
        {
            _unitOfWork = unitOfWork;
            _accommodationRepository = accommodationRepository;
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
                IsManualAccept = true
            };

            return await _accommodationRepository.Create(createAccommodation, cancellationToken);
        }
    }
}
