namespace UserService.Domain.Dtos.Identity.Role;

public record IdentityUpdateRolesDto(string IdentityId, long UserId, string Email, List<Entities.Role> NewRoles);