using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using ftrip.io.booking_service.AccommodationConfiguration.UseCases.CreateAccommodationRequests;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationConfiguration.UseCases.ReadByAccommodationId
{
    public class ReadByAccommodationIdQueryHandler : IRequestHandler<ReadByAccommodationIdQuery, Accommodation>
    {
        private readonly IAccommodationRepository _accommodationRepository;
        private readonly IMediator _mediator;

        public ReadByAccommodationIdQueryHandler(
            IAccommodationRepository accommodationRepository, 
            IMediator mediator)
        {
            _accommodationRepository = accommodationRepository;
            _mediator = mediator;
        }

        public async Task<Accommodation> Handle(ReadByAccommodationIdQuery request, CancellationToken cancellationToken)
        {
            var accommodation = await _accommodationRepository.ReadByAccommodationId(request.AccommodationId, cancellationToken);
            if (accommodation == null) 
            {
                accommodation = await CreateAccommodation(request.AccommodationId, cancellationToken);    
            }

            return accommodation;
        }

        private async Task<Accommodation> CreateAccommodation(Guid accommodationId, CancellationToken cancellationToken)
        {
            var createAccommodationRequest = new CreateAccommodationRequest()
            {
                AccommodationId = accommodationId,
            };

            return await _mediator.Send(createAccommodationRequest, cancellationToken);

        }
    }
}
