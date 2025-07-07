using FluentValidation;
using GenderHealthCare.ModelViews.ConsultationModels;

namespace GenderHealthCare.Services.Validators
{
    public class UpdateConsultationResultRequestValidator : AbstractValidator<UpdateConsultationResultRequest>
    {
        public UpdateConsultationResultRequestValidator()
        {
            RuleFor(x => x.Result).NotEmpty().MaximumLength(2000);
        }
    }
}
