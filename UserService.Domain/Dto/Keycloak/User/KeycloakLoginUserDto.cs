namespace UserService.Domain.Dto.Keycloak.User;

public record KeycloakLoginUserDto(string Username)
{
    public string Password { get; set; }
}