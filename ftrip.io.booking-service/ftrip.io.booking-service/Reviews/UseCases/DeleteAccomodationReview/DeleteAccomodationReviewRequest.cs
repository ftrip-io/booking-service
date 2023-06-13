using ftrip.io.booking_service.Reviews.Domain;
using MediatR;
using System;

namespace ftrip.io.booking_service.Reviews.UseCases.DeleteAccomodationReview
{
    public class DeleteAccomodationReviewRequest : IRequest<AccomodationReview>
    {
        public Guid ReviewId { get; set; }
    }
}