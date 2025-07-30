namespace UserService.Domain.Dtos.Identity.Role;

public record IdentityUpdateRolesDto
{
    public Guid KeycloakUserId { get; init; }
    public long UserId { get; init; }
    public string Email { get; init; }
    public List<Entities.Role> NewRoles { get; set; }
}