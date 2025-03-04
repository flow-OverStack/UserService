using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace UserService.Api.Authorization;

/// <inheritdoc />
public class AudienceAuthorizationHandler : AuthorizationHandler<AudienceRequirement>
{
    private const string AudienceClaimType = "aud";

    /// <inheritdoc />
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AudienceRequirement requirement)
    {
        var audience = context.User.FindFirstValue(AudienceClaimType);

        if (requirement.RequiredAudiences.Contains(audience))
            context.Succeed(requirement);
        else
            context.Fail();

        return Task.CompletedTask;
    }
}