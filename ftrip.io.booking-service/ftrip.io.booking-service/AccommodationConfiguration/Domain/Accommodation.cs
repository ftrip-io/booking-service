using ftrip.io.framework.Domain;
using System;
namespace ftrip.io.booking_service.AccommodationConfiguration.Domain
{
    public class Accommodation : Entity<Guid>
    {
        public Guid AccommodationId { get; set; }   
        public bool IsManualAccept { get; set; }
    }
}
