namespace UserService.Domain.Keycloak;

public class KeycloakCredentials
{
    public string Type { get; set; }
    public string Value { get; set; }
    public bool Temporary { get; set; } = false;
}