using UserService.Domain.Interfaces.Entity;

namespace UserService.Domain.Dtos.User;

public record UpdateUsernameDto(long UserId, string Username) : IValidatableUsername;