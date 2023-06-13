using FluentValidation;
using ftrip.io.booking_service.Reviews.Common.ValidationExtensions;

namespace ftrip.io.booking_service.Reviews.UseCases.UpdateHostReview
{
    public class UpdateHostReviewRequestValidator : AbstractValidator<UpdateHostReviewRequest>
    {
        public UpdateHostReviewRequestValidator()
        {
            RuleFor(r => r.CommunicationGrade).IsGrade();
            RuleFor(r => r.OverallGrade).IsGrade();
            RuleFor(r => r.RecensionText).IsRecension();
        }
    }
}