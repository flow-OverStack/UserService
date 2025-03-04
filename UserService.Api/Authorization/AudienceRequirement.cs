using Microsoft.AspNetCore.Authorization;

namespace UserService.Api.Authorization;

/// <inheritdoc />
public class AudienceRequirement(params string[] requiredAudiences) : IAuthorizationRequirement
{
    /// <summary>
    ///     The required audience for current user
    /// </summary>
    public readonly string[] RequiredAudiences = requiredAudiences;
}