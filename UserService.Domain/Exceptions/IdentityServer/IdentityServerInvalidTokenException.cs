using UserService.Domain.Enums;
using UserService.Domain.Exceptions.IdentityServer.Base;
using UserService.Domain.Resources;
using UserService.Domain.Results;

namespace UserService.Domain.Exceptions.IdentityServer;

public class IdentityServerInvalidTokenException(string identityServerName, string message)
    : IdentityServerBusinessException(identityServerName, message)
{
    public override BaseResult GetBaseResult()
    {
        return BaseResult.Failure(ErrorMessage.InvalidToken, (int)ErrorCodes.InvalidToken);
    }
}