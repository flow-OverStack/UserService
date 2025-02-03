using UserService.Domain.Keycloak;

namespace UserService.Keycloak.HttpModels;

internal sealed class UpdateUserPayload
{
    public string Email { get; set; }

    public KeycloakAttributes Attributes { get; set; }
}