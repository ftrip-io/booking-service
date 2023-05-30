using ftrip.io.booking_service.AccommodationConfiguration.UseCases.CreateAccommodationRequests;
using ftrip.io.catalog_service.contracts.Accommodations.Events;
using MassTransit;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationConfiguration.Consumers
{
    public class AccommodationCreatedEventConsumer : IConsumer<AccommodationCreatedEvent>
    {
        private readonly IMediator _mediator;

        public AccommodationCreatedEventConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<AccommodationCreatedEvent> context)
        {
            var accommodationCreated = context.Message;

            var createAccommodationRequest = new CreateAccommodationRequest()
            {
                AccommodationId = accommodationCreated.AccommodationId,
                HostId = accommodationCreated.HostId
            };

            await _mediator.Send(createAccommodationRequest, CancellationToken.None);
        }
    }
}
