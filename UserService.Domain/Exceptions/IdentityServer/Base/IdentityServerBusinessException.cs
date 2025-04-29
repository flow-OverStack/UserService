using UserService.Domain.Interfaces.Exception;
using UserService.Domain.Results;

namespace UserService.Domain.Exceptions.IdentityServer.Base;

/// <summary>
///     Represents any identity server business logic exception
/// </summary>
public abstract class IdentityServerBusinessException(string identityServerName, string message)
    : IdentityServerException(identityServerName,
        message), IBaseResultProvider
{
    public abstract BaseResult GetBaseResult();
}