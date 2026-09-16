namespace UserService.Application.Services.Provisioning;

/// <summary>
///     Resolves a unique, sanitized username for a raw (identity-server-supplied) username.
/// </summary>
public interface IUsernameGenerator
{
    Task<(string Username, bool IsTemporary)> ResolveUniqueUsernameAsync(
        string rawUsername, CancellationToken cancellationToken = default);
}
