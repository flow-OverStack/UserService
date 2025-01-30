using UserService.Domain.Keycloak;

namespace UserService.Tests.FunctionalTests.Configurations.Keycloak;

internal class KeycloakRequestUser
{
    public string Username { get; set; }
    public List<KeycloakCredential> Credentials { get; set; }
}