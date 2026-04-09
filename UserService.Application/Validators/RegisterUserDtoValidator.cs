using FluentValidation;
using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Domain.Settings;

namespace UserService.Application.Validators;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        Include(new UsernameValidator());

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ErrorMessage.InvalidEmail)
            .MaximumLength(EntityConstraints.EmailMaxLength).WithMessage(ErrorMessage.InvalidEmail)
            .EmailAddress().WithMessage(ErrorMessage.InvalidEmail);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ErrorMessage.InvalidCredentials)
            .MinimumLength(BusinessRules.PasswordMinLength).WithMessage(ErrorMessage.InvalidCredentials);
    }
}