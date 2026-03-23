using UserService.Domain.Interfaces.Validation;

namespace UserService.Domain.Dtos.User;

public record InitUserDto(string Username, string Email, string IdentityId) : IValidatableCredentials;