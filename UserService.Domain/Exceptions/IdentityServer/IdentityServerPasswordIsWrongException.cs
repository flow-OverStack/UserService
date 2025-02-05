using UserService.Domain.Enum;
using UserService.Domain.Exceptions.IdentityServer.Base;
using UserService.Domain.Resources;
using UserService.Domain.Result;

namespace UserService.Domain.Exceptions.IdentityServer;

public class IdentityServerPasswordIsWrongException : IdentityServerBusinessException
{
    public IdentityServerPasswordIsWrongException(string identityServerName, string message) :
        base(identityServerName, message)
    {
    }

    public IdentityServerPasswordIsWrongException(string identityServerName, string message, Exception innerException) :
        base(identityServerName, message, innerException)
    {
    }


    public override BaseResult GetBaseResult()
    {
        return BaseResult.Failure(ErrorMessage.PasswordIsWrong, (int)ErrorCodes.PasswordIsWrong);
    }
}