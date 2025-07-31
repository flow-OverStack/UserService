using UserService.Domain.Dtos.Identity.Role;
using UserService.Domain.Dtos.Identity.User;
using UserService.Domain.Dtos.Token;

namespace UserService.Domain.Interfaces.Identity;

/// <summary>
///     Service for user authentication in an identity server
/// </summary>
public interface IIdentityServer
{
    /// <summary>
    ///     Register user in identity server
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentityUserDto> RegisterUserAsync(IdentityRegisterUserDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Logs user in identity server
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TokenDto> LoginUserAsync(IdentityLoginUserDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Refresh user's token in identity server
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TokenDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Update user's roles in identity server
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateRolesAsync(IdentityUpdateRolesDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Rolls back the registration of a user; does not catch errors
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task RollbackRegistrationAsync(IdentityUserDto dto);

    /// <summary>
    ///     Rolls back user's roles update; does not catch errors
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task RollbackUpdateRolesAsync(IdentityUpdateRolesDto dto);
}