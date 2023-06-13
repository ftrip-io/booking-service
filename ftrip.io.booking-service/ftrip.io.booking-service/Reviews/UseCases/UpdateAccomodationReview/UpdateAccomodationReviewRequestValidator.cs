using FluentValidation;
using ftrip.io.booking_service.Reviews.Common.ValidationExtensions;

namespace ftrip.io.booking_service.Reviews.UseCases.UpdateAccomodationReview
{
    public class UpdateAccomodationReviewRequestValidator : AbstractValidator<UpdateAccomodationReviewRequest>
    {
        public UpdateAccomodationReviewRequestValidator()
        {
            RuleFor(r => r.AccomodationGrade).IsGrade();
            RuleFor(r => r.LocationGrade).IsGrade();
            RuleFor(r => r.ValueForMoneyGrade).IsGrade();
            RuleFor(r => r.RecensionText).IsRecension();
        }
    }
}