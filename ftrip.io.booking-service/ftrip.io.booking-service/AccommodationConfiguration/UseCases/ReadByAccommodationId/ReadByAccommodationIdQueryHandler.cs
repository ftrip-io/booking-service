using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationConfiguration.UseCases.ReadByAccommodationId
{
    public class ReadByAccommodationIdQueryHandler : IRequestHandler<ReadByAccommodationIdQuery, Accommodation>
    {
        private readonly IAccommodationRepository _accommodationRepository;

        public ReadByAccommodationIdQueryHandler(IAccommodationRepository accommodationRepository)
        {
            _accommodationRepository = accommodationRepository;
        }

        public async Task<Accommodation> Handle(ReadByAccommodationIdQuery request, CancellationToken cancellationToken)
        {
            return await _accommodationRepository.ReadByAccommodationId(request.AccommodationId, cancellationToken);
        }
    }
}
