using UserService.Application.Exceptions.IdentityServer.Base;

namespace UserService.Application.Exceptions.IdentityServer;

public sealed class IdentityServerInternalException(string identityServerName, string message, Exception innerException)
    : IdentityServerException(identityServerName, message, innerException);