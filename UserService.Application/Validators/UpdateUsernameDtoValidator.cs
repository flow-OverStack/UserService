using FluentValidation;
using UserService.Domain.Dtos.User;

namespace UserService.Application.Validators;

public class UpdateUsernameDtoValidator : AbstractValidator<UpdateUsernameDto>
{
    public UpdateUsernameDtoValidator()
    {
        Include(new UsernameValidator());
    }
}