using ftrip.io.booking_service.Reviews.Domain;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace ftrip.io.booking_service.Reviews.UseCases.UpdateHostReview
{
    public class UpdateHostReviewRequest : IRequest<HostReview>
    {
        [JsonIgnore]
        public Guid ReviewId { get; set; }

        public int CommunicationGrade { get; set; }
        public int OverallGrade { get; set; }

        public string RecensionText { get; set; }
    }
}