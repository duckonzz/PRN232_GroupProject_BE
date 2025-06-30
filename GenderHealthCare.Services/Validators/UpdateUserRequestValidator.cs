using FluentValidation;
using GenderHealthCare.ModelViews.AuthenticationModels;

namespace GenderHealthCare.Services.Validators
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.FullName)
                .MaximumLength(100)
                .WithMessage("Full name must not exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.FullName));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(?:\+84|0)(3|5|7|8|9)\d{8}$")
                .WithMessage("Invalid phone number format.")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today)
                .WithMessage("Date of birth cannot be in the future.")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.Gender)
                .IsInEnum()
                .WithMessage("Gender is invalid.")
                .When(x => x.Gender.HasValue);
        }
    }
}
