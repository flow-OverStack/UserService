namespace UserService.Domain.Settings;

public class KeycloakSettings
{
    public string Host { get; set; }
    public string Realm { get; set; }
    public string AdminToken { get; set; }
    public string ClientId { get; set; }
    public string Audience { get; set; }
    public string UserIdClaim { get; set; }
    public string RolesClaim { get; set; }

    public string MetadataAddress => $"{Host}/realms/{Realm}/.well-known/openid-configuration";
    public string LoginEndpoint => $"/realms/{Realm}/protocol/openid-connect/token";
    public string UsersEndpoint => $"/admin/realms/{Realm}/users";
}