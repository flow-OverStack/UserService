using UserService.Domain.Enum;
using UserService.Domain.Exceptions.IdentityServer.Base;
using UserService.Domain.Resources;
using UserService.Domain.Result;

namespace UserService.Domain.Exceptions.IdentityServer;

public class IdentityServerPasswordIsWrongException(string identityServerName, string message)
    : IdentityServerBusinessException(identityServerName, message)
{
    public override BaseResult GetBaseResult()
    {
        return BaseResult.Failure(ErrorMessage.PasswordIsWrong, (int)ErrorCodes.PasswordIsWrong);
    }
}