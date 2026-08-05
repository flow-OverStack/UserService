namespace UserService.Domain.Dtos.Identity;

/// <summary>
///     Exactly the fields IIdentityRoleSynchronizer needs to sync a user's roles to the
///     identity server - a purpose-built projection, not a hand-built partial entity that
///     silently maps any field it forgot to set to null.
/// </summary>
public record IdentitySyncSourceDto(
    long Id,
    string IdentityId,
    string Username,
    string Email,
    List<Entities.Role> Roles);