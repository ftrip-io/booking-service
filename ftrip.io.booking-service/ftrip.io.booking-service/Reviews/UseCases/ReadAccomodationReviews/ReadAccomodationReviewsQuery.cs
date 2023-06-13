using ftrip.io.booking_service.Reviews.Domain;
using ftrip.io.framework.Persistence.UtilityClasses;
using MediatR;
using System;

namespace ftrip.io.booking_service.Reviews.UseCases.ReadAccomodationReviews
{
    public class ReadAccomodationReviewsQuery : IRequest<PageResult<AccomodationReview>>
    {
        public ReadAccomodationReviewsQueryFilers Filters { get; set; }
        public Page Page { get; set; }

        public ReadAccomodationReviewsQuery()
        {
            Filters = new ReadAccomodationReviewsQueryFilers();
            Page = Page.Max;
        }
    }

    public class ReadAccomodationReviewsQueryFilers
    {
        public Guid? AccomodationId { get; set; }
        public Guid? GuestId { get; set; }

        public int? GradeFrom { get; set; }
        public int? GradeTo { get; set; }

        public string RecentionText { get; set; }
    }
}