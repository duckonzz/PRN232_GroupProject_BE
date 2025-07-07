using FluentValidation;
using GenderHealthCare.ModelViews.ConsultationModels;

namespace GenderHealthCare.Services.Validators
{
    public class ConsultationRequestValidator : AbstractValidator<ConsultationRequest>
    {
        public ConsultationRequestValidator()
        {
            RuleFor(x => x.SlotId)
                .NotEmpty().WithMessage("Slot ID is required");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Reason for consultation is required")
                .MaximumLength(1000).WithMessage("Reason must not exceed 10000 characters");


        }
    }
}
