using UserService.Domain.Enum;
using UserService.Domain.Exceptions.IdentityServer.Base;
using UserService.Domain.Resources;
using UserService.Domain.Result;

namespace UserService.Domain.Exceptions.IdentityServer;

public class IdentityServerInvalidTokenException(string identityServerName, string message)
    : IdentityServerBusinessException(identityServerName, message)
{
    public override BaseResult GetBaseResult()
    {
        return BaseResult.Failure(ErrorMessage.InvalidToken, (int)ErrorCodes.InvalidToken);
    }
}