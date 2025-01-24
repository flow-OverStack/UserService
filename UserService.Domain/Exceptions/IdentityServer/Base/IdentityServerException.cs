namespace UserService.Domain.Exceptions.IdentityServer.Base;

public abstract class IdentityServerException : Exception
{
    protected IdentityServerException(string identityServerName, string message) : base(message)
    {
        IdentityServerName = identityServerName;
    }

    protected IdentityServerException(string identityServerName, string message, Exception innerException) : base(
        message,
        innerException)
    {
        IdentityServerName = identityServerName;
    }

    public string IdentityServerName { get; set; }
}