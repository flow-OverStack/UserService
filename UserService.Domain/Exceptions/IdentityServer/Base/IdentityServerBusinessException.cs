using UserService.Domain.Interfaces.Exceptions;
using UserService.Domain.Result;

namespace UserService.Domain.Exceptions.IdentityServer.Base;

/// <summary>
///     Represents any identity server business logic exception
/// </summary>
public abstract class IdentityServerBusinessException : IdentityServerException, IBaseResultProvider
{
    protected IdentityServerBusinessException(string identityServerName, string message) : base(identityServerName,
        message)
    {
    }

    protected IdentityServerBusinessException(string identityServerName, string message, Exception innerException) :
        base(identityServerName, message, innerException)
    {
    }

    public abstract BaseResult GetBaseResult();
}