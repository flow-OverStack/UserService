using UserService.Domain.Dto.Token;
using UserService.Domain.Dto.User;
using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

/// <summary>
///     Service for user authentication
/// </summary>
public interface IAuthService
{
    /// <summary>
    ///     Registers user
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<UserDto>> RegisterAsync(RegisterUserDto dto);

    /// <summary>
    ///     Logs user in with username given
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<TokenDto>> LoginWithUsernameAsync(LoginUsernameUserDto dto);

    /// <summary>
    ///     Logs user in with email given
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<TokenDto>> LoginWithEmailAsync(LoginEmailUserDto dto);
}