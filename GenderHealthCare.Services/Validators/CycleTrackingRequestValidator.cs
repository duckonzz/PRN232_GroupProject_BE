using FluentValidation;
using GenderHealthCare.ModelViews.CycleTrackingModels;

namespace GenderHealthCare.Services.Validators
{
    public class CycleTrackingRequestValidator : AbstractValidator<CycleTrackingRequest>
    {
        public CycleTrackingRequestValidator()
        {
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required")
                .GreaterThan(DateTime.UtcNow.AddYears(-1)).WithMessage("Start date must not be too far in the past")
                .LessThan(DateTime.UtcNow.AddYears(1)).WithMessage("Start date must not be too far in the future");

            RuleFor(x => x.CycleLength)
                .GreaterThan(20).WithMessage("Cycle length must be at least 21 days")
                .LessThanOrEqualTo(35).WithMessage("Cycle length must be at most 35 days")
                .When(x => x.CycleLength.HasValue);

            RuleFor(x => x.PeriodLength)
                .GreaterThan(1).WithMessage("Period length must be at least 2 days")
                .LessThanOrEqualTo(10).WithMessage("Period length must be at most 10 days")
                .When(x => x.PeriodLength.HasValue);

            RuleFor(x => x)
                .Must(x => (!x.CycleLength.HasValue || !x.PeriodLength.HasValue) || x.CycleLength >= x.PeriodLength)
                .WithMessage("Cycle length must be greater than or equal to period length");
        }
    }
}