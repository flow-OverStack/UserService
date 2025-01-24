using UserService.Domain.Exceptions.IdentityServer.Base;

namespace UserService.Domain.Exceptions.IdentityServer;

public class IdentityServerInternalException : IdentityServerException
{
    public IdentityServerInternalException(string identityServerName, string message) : base(identityServerName,
        message)
    {
    }

    public IdentityServerInternalException(string identityServerName, string message, Exception innerException) : base(
        identityServerName, message, innerException)
    {
    }
}