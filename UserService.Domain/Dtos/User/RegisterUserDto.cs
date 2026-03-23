using UserService.Domain.Interfaces.Validation;

namespace UserService.Domain.Dtos.User;

public record RegisterUserDto(string Username, string Email, string Password) : IValidatableCredentials;