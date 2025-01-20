using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using UserService.Domain.Dto.Token;
using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface ITokenService
{
    /// <summary>
    ///     Gets principals from token
    /// </summary>
    /// <param name="token"></param>
    /// <param name="tokenValidationParameters"></param>
    /// <returns></returns>
    Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token,
        TokenValidationParameters tokenValidationParameters);

    /// <summary>
    ///     Refreshes user's access token
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<TokenDto>> RefreshToken(RefreshTokenDto dto);
}