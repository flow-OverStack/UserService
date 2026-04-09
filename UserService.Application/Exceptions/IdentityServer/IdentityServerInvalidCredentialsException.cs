using UserService.Application.Enums;
using UserService.Application.Exceptions.IdentityServer.Base;
using UserService.Application.Resources;
using UserService.Domain.Results;

namespace UserService.Application.Exceptions.IdentityServer;

public sealed class IdentityServerInvalidCredentialsException(string identityServerName, string message)
    : IdentityServerBusinessException(identityServerName, message)
{
    public override BaseResult GetBaseResult()
    {
        return BaseResult.Failure(ErrorMessage.InvalidCredentials, (int)ErrorCodes.InvalidCredentials);
    }
}