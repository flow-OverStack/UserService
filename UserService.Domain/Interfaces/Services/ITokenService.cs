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
    /// <param name="cancellationToken"></param>
    Task<BaseResult<TokenDto>> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);
}