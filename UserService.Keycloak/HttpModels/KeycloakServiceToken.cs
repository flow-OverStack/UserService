namespace UserService.Keycloak.HttpModels;

internal sealed class KeycloakServiceToken
{
    public string AccessToken { get; set; }

    public DateTime Expires { get; set; }
}