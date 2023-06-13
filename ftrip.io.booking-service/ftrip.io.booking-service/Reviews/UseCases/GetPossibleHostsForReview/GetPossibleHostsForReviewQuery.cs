using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.booking_service.Reviews.UseCases.GetPossibleHostsForReview
{
    public class GetPossibleHostsForReviewQuery : IRequest<IEnumerable<Guid>>
    {
        public Guid GuestId { get; set; }
    }
}