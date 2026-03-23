using UserService.Keycloak.KeycloakModels;

namespace UserService.Keycloak.HttpModels;

internal sealed class UpdateUserPayload
{
    public string Username { get; set; }
    public string Email { get; set; }

    public KeycloakAttributes Attributes { get; set; }
}