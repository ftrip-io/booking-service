using ftrip.io.booking_service.Reviews.UseCases.DeleteHostReview;
using ftrip.io.booking_service.Reviews.UseCases.GetPossibleHostsForReview;
using ftrip.io.booking_service.Reviews.UseCases.ReadHostReviews;
using ftrip.io.booking_service.Reviews.UseCases.ReadHostReviewsSummary;
using ftrip.io.booking_service.Reviews.UseCases.ReviewHost;
using ftrip.io.booking_service.Reviews.UseCases.UpdateHostReview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews
{
    [Route("api/hosts")]
    [ApiController]
    public class HostReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HostReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{hostId}/reviews/summary")]
        public async Task<IActionResult> ReadSummary(Guid hostId, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new ReadHostReviewsSummaryQuery() { HostId = hostId }, cancellationToken));
        }

        [HttpGet("reviews")]
        public async Task<IActionResult> QueryReviews([FromQuery] ReadHostReviewsQuery query, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(query, cancellationToken));
        }

        [Authorize(Roles = "Guest")]
        [HttpGet("reviews/possibilities")]
        public async Task<IActionResult> ReadPossibleHostsForReview([FromQuery] GetPossibleHostsForReviewQuery query, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(query, cancellationToken));
        }

        [Authorize(Roles = "Guest")]
        [HttpPost("reviews")]
        public async Task<IActionResult> CreateReview(ReviewHostRequest request, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(request, cancellationToken));
        }

        [Authorize(Roles = "Guest")]
        [HttpPut("reviews/{reviewId}")]
        public async Task<IActionResult> UpdateReview(Guid reviewId, UpdateHostReviewRequest request, CancellationToken cancellationToken = default)
        {
            request.ReviewId = reviewId;

            return Ok(await _mediator.Send(request, cancellationToken));
        }

        [Authorize(Roles = "Guest")]
        [HttpDelete("reviews/{reviewId}")]
        public async Task<IActionResult> DeleteReview(Guid reviewId, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new DeleteHostReviewRequest() { ReviewId = reviewId }, cancellationToken));
        }
    }
}