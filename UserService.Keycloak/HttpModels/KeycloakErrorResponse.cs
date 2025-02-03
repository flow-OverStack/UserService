using Newtonsoft.Json;

namespace UserService.Keycloak.HttpModels;

internal sealed class KeycloakErrorResponse
{
    [JsonProperty("error")] public string Error { get; set; }
    [JsonProperty("error_description")] public string ErrorDescription { get; set; }
}