using UserService.Application.Exceptions.IdentityServer.Base;

namespace UserService.Application.Exceptions.IdentityServer;

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