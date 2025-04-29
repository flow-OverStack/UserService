using UserService.Domain.Dtos.Token;
using UserService.Domain.Dtos.User;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

/// <summary>
///     Service for user authentication
/// </summary>
public interface IAuthService
{
    /// <summary>
    ///     Registers user
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<UserDto>> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Logs user in with username given
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<TokenDto>> LoginWithUsernameAsync(LoginUsernameUserDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Logs user in with email given
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<TokenDto>>
        LoginWithEmailAsync(LoginEmailUserDto dto, CancellationToken cancellationToken = default);
}