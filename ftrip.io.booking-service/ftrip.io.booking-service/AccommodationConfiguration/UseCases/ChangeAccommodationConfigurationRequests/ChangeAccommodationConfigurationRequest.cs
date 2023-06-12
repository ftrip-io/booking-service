using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace ftrip.io.booking_service.AccommodationConfiguration.UseCases.ChangeAccommodationConfigurationRequests
{
    public class ChangeAccommodationConfigurationRequest : IRequest<Accommodation>
    {
        [JsonIgnore]
        public Guid AccommodationId { get; set; }
        public bool IsManualAccept { get; set; }    
    }
}
