namespace UserService.Domain.Settings;

public class KeycloakSettings
{
    public string Url { get; set; }
    public string Realm { get; set; }
    public string AdminToken { get; set; }
    public string ClientId { get; set; }
    public string Audience { get; set; }
    public string UserIdAttributeName { get; set; }
    public string RolesAttributeName { get; set; }
    public string MetadataAddress => $"{Url}/realms/{Realm}/.well-known/openid-configuration";
    public string LoginUrl => $"{Url}/realms/{Realm}/protocol/openid-connect/token";
    public string UsersUrl => $"{Url}/admin/realms/{Realm}/users";
}