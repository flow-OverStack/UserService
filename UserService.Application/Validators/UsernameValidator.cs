using FluentValidation;
using UserService.Application.Resources;
using UserService.Domain.Interfaces.Entity;
using UserService.Domain.Settings;

namespace UserService.Application.Validators;

public class UsernameValidator : AbstractValidator<IValidatableUsername>
{
    public UsernameValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(ErrorMessage.InvalidUsername)
            .MaximumLength(EntityConstraints.UsernameMaxLength).WithMessage(ErrorMessage.InvalidUsername)
            .Matches("^[a-z0-9_.\\-]+$").WithMessage(ErrorMessage.InvalidUsername);
    }
}