using Swashbuckle.AspNetCore.Filters;
using System;

namespace ftrip.io.booking_service.Reviews.UseCases.ReviewAccomodation
{
    public class ReviewAccomodationRequestExample : IExamplesProvider<ReviewAccomodationRequest>
    {
        public ReviewAccomodationRequest GetExamples()
        {
            return new ReviewAccomodationRequest()
            {
                GuestId = Guid.NewGuid(),
                AccomodationId = Guid.NewGuid(),
                AccomodationGrade = 2,
                LocationGrade = 3,
                ValueForMoneyGrade = 2,
                RecensionText = "Not that great in the end."
            };
        }
    }
}