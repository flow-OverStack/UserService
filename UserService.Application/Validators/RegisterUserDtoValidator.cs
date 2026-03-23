using FluentValidation;
using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Domain.Settings;

namespace UserService.Application.Validators;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        Include(new UserCredentialsValidator());

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ErrorMessage.InvalidPassword)
            .MinimumLength(BusinessRules.PasswordMinLength).WithMessage(ErrorMessage.InvalidPassword);
    }
}