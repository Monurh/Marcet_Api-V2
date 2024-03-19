using FluentValidation;
using Marcet_Api.Models.Dto.Auth;

namespace Marcet_Api.Validators.Auth
{
    public class RegistrationDTOValidator : AbstractValidator<RegistrationDTO>
    {
            public RegistrationDTOValidator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match.");
            }
    }
}