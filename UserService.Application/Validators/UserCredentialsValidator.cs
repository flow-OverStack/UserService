using FluentValidation;
using UserService.Application.Resources;
using UserService.Domain.Interfaces.Validation;
using UserService.Domain.Settings;

namespace UserService.Application.Validators;

public class UserCredentialsValidator : AbstractValidator<IValidatableCredentials>
{
    public UserCredentialsValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(ErrorMessage.InvalidUsername)
            .MaximumLength(EntityConstraints.UsernameMaxLength).WithMessage(ErrorMessage.InvalidUsername)
            .Matches("^[a-z0-9_.\\-]+$").WithMessage(ErrorMessage.InvalidUsername);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ErrorMessage.InvalidEmail)
            .MaximumLength(EntityConstraints.EmailMaxLength).WithMessage(ErrorMessage.InvalidEmail)
            .EmailAddress().WithMessage(ErrorMessage.InvalidEmail);
    }
}