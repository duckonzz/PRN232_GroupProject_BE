using FluentValidation;
using GenderHealthCare.Core.Enums;
using GenderHealthCare.Core.Helpers;
using GenderHealthCare.ModelViews.AuthenticationModels;

namespace GenderHealthCare.Services.Validators
{
    public class UserRegistrationRequestValidator : AbstractValidator<UserRegistrationRequest>
    {
        public UserRegistrationRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^(?:\+84|0)(3|5|7|8|9)\d{8}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required")
                .Equal(x => x.Password).WithMessage("Passwords do not match");

            RuleFor(x => x.Password)
                .Must(PasswordHelper.IsStrongPassword)
                .WithMessage("Password must be at least 8 characters and include uppercase, lowercase, number and special character");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(DateTime.Today).WithMessage("Date of birth cannot be in the future.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Gender is invalid");

            RuleFor(x => x.Role)
                .Must(r => r == Role.Customer || r == Role.Consultant)
                .WithMessage("Role must be Customer or Consultant.");
        }
    }
}
