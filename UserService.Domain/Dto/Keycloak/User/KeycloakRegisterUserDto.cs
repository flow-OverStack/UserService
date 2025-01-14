namespace UserService.Domain.Dto.Keycloak.User;

public record KeycloakRegisterUserDto(long Id, string Username, string Email, string Password, List<Entity.Role> Roles);