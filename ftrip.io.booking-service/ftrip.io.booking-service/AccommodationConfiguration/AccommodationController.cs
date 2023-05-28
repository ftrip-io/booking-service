using ftrip.io.booking_service.AccommodationConfiguration.UseCases.ChangeAccommodationConfigurationRequests;
using ftrip.io.booking_service.AccommodationConfiguration.UseCases.ReadByAccommodationId;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationConfiguration
{
    [Route("api/accommodations")]
    [ApiController]
    public class AccommodationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccommodationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{accommodationId}")]
        public async Task<IActionResult> Read(Guid accommodationId, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new ReadByAccommodationIdQuery() { AccommodationId = accommodationId }, cancellationToken));
        }

        [HttpPut("{accommodationId}")]
        public async Task<IActionResult> Configure(Guid accommodationId, ChangeAccommodationConfigurationRequest request, CancellationToken cancellationToken = default)
        {
            request.AccommodationId = accommodationId;

            return Ok(await _mediator.Send(request, cancellationToken));
        }
    }
}
