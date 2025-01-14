namespace UserService.Domain.Settings;

public class KeycloakSettings
{
    public string Url { get; set; }
    public string Realm { get; set; }
    public string AdminToken { get; set; }
    public string ClientId { get; set; }
    public string MetadataAddress { get; set; }

    public string LoginUrl { get; set; }
    public string Audience { get; set; }

    public string UserIdAttributeName { get; set; }

    public string RolesAttributeName { get; set; }

    public string UsersUrl { get; set; }
}