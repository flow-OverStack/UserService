using UserService.Domain.Dto.Token;
using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface ITokenService
{
    /// <summary>
    ///     Refreshes user's access token
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<TokenDto>> RefreshToken(RefreshTokenDto dto);
}