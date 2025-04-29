using UserService.Keycloak.KeycloakModels;

namespace UserService.Keycloak.HttpModels;

internal sealed class RegisterUserPayload
{
    public string Username { get; set; }
    public string Email { get; set; }

    public bool Enabled { get; set; } = true;

    public List<KeycloakCredential> Credentials { get; set; }
    public KeycloakAttributes Attributes { get; set; }
}