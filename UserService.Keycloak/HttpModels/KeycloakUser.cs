using Newtonsoft.Json;

namespace UserService.Keycloak.HttpModels;

internal sealed class KeycloakUser
{
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("username")] public string Username { get; set; }
}