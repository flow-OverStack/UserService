using FluentValidation;
using UserService.Domain.Dtos.User;

namespace UserService.Application.Validators;

public class InitUserDtoValidator : AbstractValidator<InitUserDto>
{
    public InitUserDtoValidator()
    {
        Include(new UserCredentialsValidator());
    }
}