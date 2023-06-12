using ftrip.io.booking_service.ReservationRequests.UseCases.AcceptReservationRequests;
using ftrip.io.booking_service.ReservationRequests.UseCases.CreateReservationRequests;
using ftrip.io.booking_service.ReservationRequests.UseCases.DeclineReservationRequests;
using ftrip.io.booking_service.ReservationRequests.UseCases.DeleteReservationRequests;
using ftrip.io.booking_service.ReservationRequests.UseCases.ReadReservationRequest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.ReservationRequests
{
    [ApiController]
    [Route("api/reservation-requests")]
    public class ReservationRequestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReservationRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Guest, Host")]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] ReadReservationRequestQuery query, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(query, cancellationToken));
        }

        [Authorize(Roles = "Guest")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateReservationRequest request, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(request, cancellationToken));
        }

        [Authorize(Roles = "Host")]
        [HttpPut("{requestId}/accept")]
        public async Task<IActionResult> Accept(Guid requestId, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new AcceptReservationRequest() { ReservationRequestId = requestId }, cancellationToken));
        }

        [Authorize(Roles = "Host")]
        [HttpPut("{requestId}/decline")]
        public async Task<IActionResult> Decline(Guid requestId, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new DeclineReservationRequest() { ReservationRequestId = requestId }, cancellationToken));
        }

        [Authorize(Roles = "Guest")]
        [HttpDelete("{requestId}")]
        public async Task<IActionResult> Delete(Guid requestId, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new DeleteReservationRequest() { ReservationRequestId = requestId }, cancellationToken));
        }
    }
}