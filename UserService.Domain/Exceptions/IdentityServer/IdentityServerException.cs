namespace UserService.Domain.Exceptions.IdentityServer;

public class IdentityServerException : Exception
{
    public IdentityServerException(string identityServerName, string message) : base(message)
    {
        IdentityServerName = identityServerName;
    }

    public IdentityServerException(string identityServerName, string message, Exception innerException) : base(message,
        innerException)
    {
        IdentityServerName = identityServerName;
    }

    public string IdentityServerName { get; set; }
}