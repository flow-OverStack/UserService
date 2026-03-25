namespace UserService.Api.Dtos.User;

/// <summary>
///     Request DTO for updating a user's username
/// </summary>
/// <param name="Username">The new username</param>
public record RequestUpdateUsernameDto(string Username);