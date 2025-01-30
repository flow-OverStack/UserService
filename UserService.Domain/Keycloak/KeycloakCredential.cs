namespace UserService.Domain.Keycloak;

public class KeycloakCredential
{
    public string Type { get; set; }
    public string Value { get; set; }
    public bool Temporary { get; set; } = false;
}