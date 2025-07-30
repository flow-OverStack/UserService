using UserService.Application.Enums;
using UserService.Application.Exceptions.IdentityServer.Base;
using UserService.Application.Resources;
using UserService.Domain.Results;

namespace UserService.Application.Exceptions.IdentityServer;

public class IdentityServerInvalidTokenException(string identityServerName, string message)
    : IdentityServerBusinessException(identityServerName, message)
{
    public override BaseResult GetBaseResult()
    {
        return BaseResult.Failure(ErrorMessage.InvalidToken, (int)ErrorCodes.InvalidToken);
    }
}