using Swashbuckle.AspNetCore.Filters;
using System;

namespace ftrip.io.booking_service.Reviews.UseCases.UpdateAccomodationReview
{
    public class UpdateAccomodationReviewRequestExample : IExamplesProvider<UpdateAccomodationReviewRequest>
    {
        public UpdateAccomodationReviewRequest GetExamples()
        {
            return new UpdateAccomodationReviewRequest()
            {
                ReviewId = Guid.NewGuid(),
                AccomodationGrade = 4,
                LocationGrade = 3,
                ValueForMoneyGrade = 4,
                RecensionText = "Edit: Went there again. It definitely was better, but still not great."
            };
        }
    }
}