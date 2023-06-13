using FluentValidation;
using ftrip.io.booking_service.Reviews.Common.ValidationExtensions;

namespace ftrip.io.booking_service.Reviews.UseCases.ReviewHost
{
    public class ReviewHostRequestValidator : AbstractValidator<ReviewHostRequest>
    {
        public ReviewHostRequestValidator()
        {
            RuleFor(r => r.CommunicationGrade).IsGrade();
            RuleFor(r => r.OverallGrade).IsGrade();
            RuleFor(r => r.RecensionText).IsRecension();
        }
    }
}