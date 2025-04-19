using UserService.Domain.Dto.Keycloak.Roles;
using UserService.Domain.Dto.Keycloak.User;
using UserService.Domain.Dto.Token;

namespace UserService.Domain.Interfaces.Services;

/// <summary>
///     Service for user authentication in keycloak
/// </summary>
public interface IIdentityServer
{
    /// <summary>
    ///     Register user in keycloak
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<KeycloakUserDto> RegisterUserAsync(KeycloakRegisterUserDto dto);

    /// <summary>
    ///     Login user in keycloak
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<TokenDto> LoginUserAsync(KeycloakLoginUserDto dto);

    /// <summary>
    ///     Refresh user's token in keycloak
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<TokenDto> RefreshTokenAsync(RefreshTokenDto dto);

    /// <summary>
    ///     Update user's roles
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task UpdateRolesAsync(KeycloakUpdateRolesDto dto);

    /// <summary>
    ///     Rolls back the registration of a user; does not catch errors
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task RollbackRegistrationAsync(Guid userId);

    /// <summary>
    ///     Rolls back the user's roles update; does not catch errors
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task RollbackUpdateRolesAsync(KeycloakUpdateRolesDto dto);
}