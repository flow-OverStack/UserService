using UserService.Domain.Enums;
using UserService.Domain.Exceptions.IdentityServer.Base;
using UserService.Domain.Resources;
using UserService.Domain.Results;

namespace UserService.Domain.Exceptions.IdentityServer;

public class IdentityServerPasswordIsWrongException(string identityServerName, string message)
    : IdentityServerBusinessException(identityServerName, message)
{
    public override BaseResult GetBaseResult()
    {
        return BaseResult.Failure(ErrorMessage.PasswordIsWrong, (int)ErrorCodes.PasswordIsWrong);
    }
}