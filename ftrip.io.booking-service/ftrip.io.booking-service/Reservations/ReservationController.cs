using ftrip.io.booking_service.Reservations.UseCases.CancelReservation;
using ftrip.io.booking_service.Reservations.UseCases.CountActiveReservations;
using ftrip.io.booking_service.Reservations.UseCases.CountActiveReservationsForHost;
using ftrip.io.booking_service.Reservations.UseCases.ReadReservation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] ReadReservationQuery query, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(query, cancellationToken));
        }

        [Authorize(Roles = "Guest")]
        [HttpGet("active/guests/{guestId}/count")]
        public async Task<IActionResult> CountActiveForGuest(Guid guestId, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new CountActiveReservationsForGuestQuery() { GuestId = guestId }, cancellationToken));
        }

        [Authorize(Roles = "Host")]
        [HttpGet("active/hosts/{hostId}/count")]
        public async Task<IActionResult> CountActiveForHost(Guid hostId, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new CountActiveReservationsForHostQuery() { HostId = hostId }, cancellationToken));
        }

        [Authorize(Roles = "Guest")]
        [HttpPut("{reservationId}")]
        public async Task<IActionResult> Cancel(Guid reservationId, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new CancelReservationRequest() { ReservationId = reservationId }, cancellationToken));
        }
    }
}