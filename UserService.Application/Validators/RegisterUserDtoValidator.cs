using FluentValidation;
using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Domain.Settings;

namespace UserService.Application.Validators;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(ErrorMessage.InvalidUsername)
            .MaximumLength(EntityConstraints.UsernameMaxLength).WithMessage(ErrorMessage.InvalidUsername)
            .Matches("^[a-z0-9_.\\-]+$").WithMessage(ErrorMessage.InvalidUsername);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ErrorMessage.InvalidEmail)
            .MaximumLength(EntityConstraints.EmailMaxLength).WithMessage(ErrorMessage.InvalidEmail)
            .EmailAddress().WithMessage(ErrorMessage.InvalidEmail);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ErrorMessage.InvalidPassword)
            .MinimumLength(BusinessRules.PasswordMinLength).WithMessage(ErrorMessage.InvalidPassword);
    }
}