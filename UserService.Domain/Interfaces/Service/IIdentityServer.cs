using UserService.Domain.Dtos.Keycloak.Role;
using UserService.Domain.Dtos.Keycloak.User;
using UserService.Domain.Dtos.Token;

namespace UserService.Domain.Interfaces.Service;

/// <summary>
///     Service for user authentication in keycloak
/// </summary>
public interface IIdentityServer
{
    /// <summary>
    ///     Register user in identity server
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<KeycloakUserDto> RegisterUserAsync(KeycloakRegisterUserDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Logs user in identity server
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TokenDto> LoginUserAsync(KeycloakLoginUserDto dto, CancellationToken cancellationToken = default);

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
    Task UpdateRolesAsync(KeycloakUpdateRolesDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Rolls back the registration of a user; does not catch errors
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task RollbackRegistrationAsync(Guid userId);

    /// <summary>
    ///     Rolls back user's roles update; does not catch errors
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task RollbackUpdateRolesAsync(KeycloakUpdateRolesDto dto);
}