using ftrip.io.booking_service.Reviews.Domain;
using MediatR;
using System;

namespace ftrip.io.booking_service.Reviews.UseCases.ReadAccomodationReviewsSummary
{
    public class ReadAccomodationReviewsSummaryQuery : IRequest<AccomodationReviewsSummary>
    {
        public Guid AccomodationId { get; set; }
    }
}