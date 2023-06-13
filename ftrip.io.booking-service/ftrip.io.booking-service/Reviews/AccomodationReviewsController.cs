using ftrip.io.booking_service.Reviews.UseCases.DeleteAccomodationReview;
using ftrip.io.booking_service.Reviews.UseCases.GetPossibleAccommodationsForReview;
using ftrip.io.booking_service.Reviews.UseCases.ReadAccomodationReviews;
using ftrip.io.booking_service.Reviews.UseCases.ReadAccomodationReviewsSummary;
using ftrip.io.booking_service.Reviews.UseCases.ReviewAccomodation;
using ftrip.io.booking_service.Reviews.UseCases.UpdateAccomodationReview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.booking_service.Reviews
{
    [Route("api/accommodations")]
    [ApiController]
    public class AccomodationReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccomodationReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{accommodationId}/reviews/summary")]
        public async Task<IActionResult> ReadSummary(Guid accommodationId, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new ReadAccomodationReviewsSummaryQuery() { AccomodationId = accommodationId }, cancellationToken));
        }

        [HttpGet("reviews")]
        public async Task<IActionResult> QueryReviews([FromQuery] ReadAccomodationReviewsQuery query, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(query, cancellationToken));
        }

        [Authorize(Roles = "Guest")]
        [HttpGet("reviews/possibilities")]
        public async Task<IActionResult> ReadPossibleAccomodationsForReview([FromQuery] GetPossibleAccommodationsForReviewQuery query, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(query, cancellationToken));
        }

        [Authorize(Roles = "Guest")]
        [HttpPost("reviews")]
        public async Task<IActionResult> CreateReview(ReviewAccomodationRequest request, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(request, cancellationToken));
        }

        [Authorize(Roles = "Guest")]
        [HttpPut("reviews/{reviewId}")]
        public async Task<IActionResult> UpdateReview(Guid reviewId, UpdateAccomodationReviewRequest request, CancellationToken cancellationToken = default)
        {
            request.ReviewId = reviewId;

            return Ok(await _mediator.Send(request, cancellationToken));
        }

        [Authorize(Roles = "Guest")]
        [HttpDelete("reviews/{reviewId}")]
        public async Task<IActionResult> DeleteReview(Guid reviewId, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new DeleteAccomodationReviewRequest() { ReviewId = reviewId }, cancellationToken));
        }
    }
}