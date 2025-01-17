namespace UserService.Domain.Dto.Keycloak.Roles;

public record KeycloakUpdateRolesDto
{
    public Guid KeycloakUserId { get; init; }
    public long UserId { get; init; }
    public string Email { get; init; }
    public List<Entity.Role> NewRoles { get; init; }
}