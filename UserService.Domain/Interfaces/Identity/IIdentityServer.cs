using UserService.Domain.Dtos.Identity;
using UserService.Domain.Dtos.Token;

namespace UserService.Domain.Interfaces.Identity;

/// <summary>
///     Service for user authentication in an identity server.
/// </summary>
public interface IIdentityServer
{
    /// <summary>
    ///     Registers a user in the identity server.
    /// </summary>
    Task<IdentityUserDto> RegisterUserAsync(IdentityRegisterUserDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Searches for an existing user by username, then by email as a fallback.
    ///     Pass null to skip searching by that field.
    ///     Returns null if no matching user is found.
    /// </summary>
    Task<IdentityUserDto?> FindUserAsync(string? username, string? email,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Logs a user in via the identity server.
    /// </summary>
    Task<TokenDto> LoginUserAsync(IdentityLoginUserDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Refreshes a user's token via the identity server.
    /// </summary>
    Task<TokenDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates a user's data in the identity server.
    /// </summary>
    Task UpdateUserAsync(IdentityUpdateUserDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a user from the identity server.
    ///     Idempotent: 404 is treated as success.
    ///     Throws on any other error so that Hangfire can retry the job.
    /// </summary>
    Task DeleteUserAsync(IdentityUserIdDto dto);
}