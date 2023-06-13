using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.framework.Persistence.UtilityClasses;
using MediatR;
using System;

namespace ftrip.io.booking_service.Reviews.UseCases.ReadHostReviews
{
    public class ReadHostReviewsQuery : IRequest<PageResult<HostReview>>
    {
        public ReadAccomodationReviewsQueryFilers Filters { get; set; }
        public Page Page { get; set; }

        public ReadHostReviewsQuery()
        {
            Filters = new ReadAccomodationReviewsQueryFilers();
            Page = Page.Max;
        }
    }

    public class ReadAccomodationReviewsQueryFilers
    {
        public Guid? HostId { get; set; }
        public Guid? GuestId { get; set; }

        public int? GradeFrom { get; set; }
        public int? GradeTo { get; set; }

        public string RecentionText { get; set; }
    }
}