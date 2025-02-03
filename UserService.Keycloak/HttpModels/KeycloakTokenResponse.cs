using Newtonsoft.Json;

namespace UserService.Keycloak.HttpModels;

internal sealed class KeycloakTokenResponse
{
    [JsonProperty("access_token")] public string AccessToken { get; set; }
    [JsonProperty("refresh_token")] public string RefreshToken { get; set; }
    [JsonProperty("expires_in")] public int AccessExpiresIn { get; set; }
    [JsonProperty("refresh_expires_in")] public int RefreshExpiresIn { get; set; }

    public bool IsValid()
    {
        return AccessToken != null
               && RefreshToken != null
               && AccessExpiresIn > 0
               && RefreshExpiresIn > 0;
    }
}