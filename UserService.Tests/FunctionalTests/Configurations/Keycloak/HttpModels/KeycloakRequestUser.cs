using UserService.Keycloak.KeycloakModels;

namespace UserService.Tests.FunctionalTests.Configurations.Keycloak.HttpModels;

internal class KeycloakRequestUser
{
    public string Username { get; set; }
    public List<KeycloakCredential> Credentials { get; set; }
}