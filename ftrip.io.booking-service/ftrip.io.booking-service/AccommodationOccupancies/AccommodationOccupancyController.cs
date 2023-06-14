using ftrip.io.booking_service.AccommodationOccupancies.UseCases.CheckAvailability;
using ftrip.io.booking_service.AccommodationOccupancies.UseCases.ReadOccupancy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.AccommodationOccupancies
{
    [ApiController]
    [Route("api/accommodation-occupancy")]
    public class AccommodationOccupancyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccommodationOccupancyController(IMediator mediator)
        {
            _mediator = mediator;   
        }

        [HttpGet]
        public async Task<IActionResult> CheckOccupancy([FromQuery] ReadOccupancyQuery query, CancellationToken cancellationToken) 
        {
            return Ok(await _mediator.Send(query, cancellationToken));
        }

        [HttpGet("availability")]
        public async Task<IActionResult> AvailableAccommodations([FromQuery] CheckAvailabilityQuery query, CancellationToken cancellationToken) 
        {
            return Ok(await _mediator.Send(query, cancellationToken));
        }
    }
}
