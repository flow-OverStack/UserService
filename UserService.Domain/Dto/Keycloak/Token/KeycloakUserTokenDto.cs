namespace UserService.Domain.Dto.Keycloak.Token;

public class KeycloakUserTokenDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    public DateTime Expires { get; set; }
}