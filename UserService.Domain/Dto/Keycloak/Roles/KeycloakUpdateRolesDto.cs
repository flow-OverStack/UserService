namespace UserService.Domain.Dto.Keycloak.Roles;

public record KeycloakUpdateRolesDto(Guid keycloakUserId, List<Entity.Role> newRoles);