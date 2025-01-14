namespace UserService.Domain.Dto.Keycloak.User;

public record KeycloakRegisterUserDto(long Id, string Username, string Email, List<Entity.Role> Roles)
{
    public string Password { get; set; } //setter is here because mapper gets hashed password
}