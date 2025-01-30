namespace UserService.Tests.Configurations.TestDbContexts;

public class KeycloakUser
{
    public Guid Id { get; set; }
    public string Username { get; set; }

    public string Password { get; set; } //for exception when password is wrong
}