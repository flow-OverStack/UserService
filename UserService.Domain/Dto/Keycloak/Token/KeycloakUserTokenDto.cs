namespace UserService.Domain.Dto.Keycloak.Token;

public class KeycloakUserTokenDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    public DateTime AccessExpires { get; set; }

    public DateTime RefreshExpires { get; set; }
}