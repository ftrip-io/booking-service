using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using MediatR;
using System;

namespace ftrip.io.booking_service.AccommodationConfiguration.UseCases.CreateAccommodationRequests
{
    public class CreateAccommodationRequest : IRequest<Accommodation>
    {
        public Guid AccommodationId { get; set; }
    }
}
