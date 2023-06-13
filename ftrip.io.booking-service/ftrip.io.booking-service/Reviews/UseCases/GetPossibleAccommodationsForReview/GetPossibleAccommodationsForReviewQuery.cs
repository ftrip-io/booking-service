using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.booking_service.Reviews.UseCases.GetPossibleAccommodationsForReview
{
    public class GetPossibleAccommodationsForReviewQuery : IRequest<IEnumerable<Guid>>
    {
        public Guid GuestId { get; set; }
    }
}