namespace UserService.Domain.Dtos.Keycloak.Role;

public record KeycloakUpdateRolesDto
{
    public Guid KeycloakUserId { get; init; }
    public long UserId { get; init; }
    public string Email { get; init; }
    public List<Entities.Role> NewRoles { get; set; }
}