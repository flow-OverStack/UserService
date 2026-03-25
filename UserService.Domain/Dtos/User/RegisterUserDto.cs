using UserService.Domain.Interfaces.Entity;

namespace UserService.Domain.Dtos.User;

public record RegisterUserDto(string Username, string Email, string Password) : IValidatableUsername;