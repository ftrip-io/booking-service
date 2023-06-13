using FluentValidation;

namespace ftrip.io.booking_service.Reviews.Common.ValidationExtensions
{
    public static class GradeValidation
    {
        public static IRuleBuilderOptions<T, int> IsGrade<T>(this IRuleBuilder<T, int> rule)
        {
            return rule.InclusiveBetween(1, 5)
                       .WithMessage("{PropertyName} must be between 1 and 5.");
        }
    }
}