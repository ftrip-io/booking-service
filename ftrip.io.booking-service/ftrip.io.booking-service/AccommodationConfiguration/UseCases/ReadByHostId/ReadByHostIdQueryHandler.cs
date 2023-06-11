using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationConfiguration.UseCases.ReadByHostId
{
    public class ReadByHostIdQueryHandler : IRequestHandler<ReadByHostIdQuery, IEnumerable<Accommodation>>
    {
        private readonly IAccommodationRepository _accommodationRepository;

        public ReadByHostIdQueryHandler(IAccommodationRepository accommodationRepository)
        {
            _accommodationRepository = accommodationRepository;
        }

        public async Task<IEnumerable<Accommodation>> Handle(ReadByHostIdQuery request, CancellationToken cancellationToken)
        {
            return await _accommodationRepository.ReadByHostId(request.HostId, cancellationToken);
        }
    }
}
