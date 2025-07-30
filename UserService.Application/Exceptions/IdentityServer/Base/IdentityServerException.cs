namespace UserService.Application.Exceptions.IdentityServer.Base;

/// <summary>
///     Represents any identity server error
/// </summary>
public abstract class IdentityServerException : Exception
{
    protected IdentityServerException(string identityServerName, string message) : base(message)
    {
        IdentityServerName = identityServerName;
    }

    protected IdentityServerException(string identityServerName, string message, Exception innerException) : base(
        message, innerException)
    {
        IdentityServerName = identityServerName;
    }

    public string IdentityServerName { get; set; }
}