using System.Security.Claims;
using UserService.Domain.Dto.Keycloak.Roles;
using UserService.Domain.Dto.Keycloak.Token;
using UserService.Domain.Dto.Keycloak.User;

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
    Task<KeycloakUserTokenDto> LoginUserAsync(KeycloakLoginUserDto dto);

    /// <summary>
    ///     Refresh user's token in keycloak
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<KeycloakUserTokenDto> RefreshTokenAsync(KeycloakRefreshTokenDto dto);

    /// <summary>
    ///     Update user's roles
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task UpdateRolesAsync(KeycloakUpdateRolesDto dto);

    /// <summary>
    ///     Gets principals from token
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    Task<ClaimsPrincipal> GetPrincipalFromExpiredTokenAsync(string token);
}