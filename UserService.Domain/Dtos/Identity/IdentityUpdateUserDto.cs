namespace UserService.Domain.Dtos.Identity;

public record IdentityUpdateUserDto(
    string IdentityId,
    string Username,
    long UserId,
    string Email,
    List<Entities.Role> Roles);