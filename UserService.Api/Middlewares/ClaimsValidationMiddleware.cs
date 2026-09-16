using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;

namespace UserService.Api.Middlewares;

public class ClaimsValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity is { IsAuthenticated: true }) // if controller requires authorization
        {
            if (RequiredClaimsExists(context))
            {
                await next(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = MediaTypeNames.Text.Plain;
                await context.Response.WriteAsync("Invalid claims");
            }
        }
        else
        {
            await next(context);
        }
    }

    private static bool RequiredClaimsExists(HttpContext context)
    {
        var user = context.User;

        return user.HasClaim(c => c.Type == ClaimTypes.NameIdentifier)
               && !string.IsNullOrWhiteSpace(user.FindFirstValue(JwtRegisteredClaimNames.PreferredUsername))
               && !string.IsNullOrWhiteSpace(user.FindFirstValue(JwtRegisteredClaimNames.Email))
               && !string.IsNullOrWhiteSpace(user.FindFirstValue(JwtRegisteredClaimNames.Sub))
               && user.HasClaim(c => c.Type == ClaimTypes.Role);
    }
}
