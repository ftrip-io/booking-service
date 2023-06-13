using ftrip.io.booking_service.Reviews.Domain;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace ftrip.io.booking_service.Reviews.UseCases.UpdateAccomodationReview
{
    public class UpdateAccomodationReviewRequest : IRequest<AccomodationReview>
    {
        [JsonIgnore]
        public Guid ReviewId { get; set; }

        public int AccomodationGrade { get; set; }
        public int LocationGrade { get; set; }
        public int ValueForMoneyGrade { get; set; }

        public string RecensionText { get; set; }
    }
}