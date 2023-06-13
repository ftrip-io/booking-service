using Swashbuckle.AspNetCore.Filters;
using System;

namespace ftrip.io.booking_service.Reviews.UseCases.ReviewHost
{
    public class ReviewHostRequestExample : IExamplesProvider<ReviewHostRequest>
    {
        public ReviewHostRequest GetExamples()
        {
            return new ReviewHostRequest()
            {
                GuestId = Guid.NewGuid(),
                HostId = Guid.NewGuid(),
                CommunicationGrade = 4,
                OverallGrade = 5,
                RecensionText = "Really great host."
            };
        }
    }
}