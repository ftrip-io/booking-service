using ftrip.io.booking_service.Reviews.Domain;
using MediatR;
using System;

namespace ftrip.io.booking_service.Reviews.UseCases.ReadHostReviewsSummary
{
    public class ReadHostReviewsSummaryQuery : IRequest<HostReviewsSummary>
    {
        public Guid HostId { get; set; }
    }
}