using Microsoft.AspNetCore.Authorization;

namespace UserService.Api.Authorization;

/// <inheritdoc />
public class AudienceRequirement(string requiredAudience) : IAuthorizationRequirement
{
    /// <summary>
    ///     The required audience for current user
    /// </summary>
    public string RequiredAudience { get; } = requiredAudience;
}