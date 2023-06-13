using FluentValidation;

namespace ftrip.io.booking_service.Reviews.Common.ValidationExtensions
{
    public static class RecensionValidation
    {
        public static IRuleBuilderOptions<T, string> IsRecension<T>(this IRuleBuilder<T, string> rule)
        {
            return rule.NotNull()
                       .MinimumLength(10)
                       .WithMessage("{PropertyName} must have at least 10 characters.");
        }
    }
}