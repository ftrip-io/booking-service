using ftrip.io.booking_service.AccommodationConfiguration.Domain;
using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.booking_service.AccommodationConfiguration.UseCases.ReadByHostId
{
    public class ReadByHostIdQuery : IRequest<IEnumerable<Accommodation>>
    {
        public Guid HostId { get; set; }
    }
}
