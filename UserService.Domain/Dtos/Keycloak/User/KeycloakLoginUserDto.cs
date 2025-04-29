namespace UserService.Domain.Dtos.Keycloak.User;

public record KeycloakLoginUserDto(string Username)
{
    public string Password { get; set; }
}