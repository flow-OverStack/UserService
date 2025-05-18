using UserService.Domain.Dtos.Token;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

public interface ITokenService
{
    /// <summary>
    ///     Refreshes user's token
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <param name="cancellationToken"></param>
    Task<BaseResult<TokenDto>> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);
}