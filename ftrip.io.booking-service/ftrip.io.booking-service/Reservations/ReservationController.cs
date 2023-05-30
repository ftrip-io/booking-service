using ftrip.io.booking_service.Reservations.UseCases.CancelReservation;
using ftrip.io.booking_service.Reservations.UseCases.ReadReservation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reservations
{
    [Route("api/reservations")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReservationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] ReadReservationQuery query, CancellationToken cancelationToken = default)
        {
            return Ok(await _mediator.Send(query, cancelationToken));
        }

        [HttpPut("{reservationId}")]
        public async Task<IActionResult> Cancel(Guid reservationId, CancellationToken cancellationToken = default)
        {

            return Ok(await _mediator.Send(new CancelReservationRequest() { ReservationId = reservationId }, cancellationToken));
        }
    }
}
