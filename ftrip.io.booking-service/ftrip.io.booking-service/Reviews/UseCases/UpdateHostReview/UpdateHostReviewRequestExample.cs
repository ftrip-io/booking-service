using Swashbuckle.AspNetCore.Filters;
using System;

namespace ftrip.io.booking_service.Reviews.UseCases.UpdateHostReview
{
    public class UpdateHostReviewRequestExample : IExamplesProvider<UpdateHostReviewRequest>
    {
        public UpdateHostReviewRequest GetExamples()
        {
            return new UpdateHostReviewRequest()
            {
                ReviewId = Guid.NewGuid(),
                CommunicationGrade = 2,
                OverallGrade = 2,
                RecensionText = "Edit: Went there again. I don't know what happened but host was so uncomfortable."
            };
        }
    }
}