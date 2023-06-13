using ftrip.io.booking_service.Reviews.Domain;
using MediatR;
using System;

namespace ftrip.io.booking_service.Reviews.UseCases.DeleteHostReview
{
    public class DeleteHostReviewRequest : IRequest<HostReview>
    {
        public Guid ReviewId { get; set; }
    }
}