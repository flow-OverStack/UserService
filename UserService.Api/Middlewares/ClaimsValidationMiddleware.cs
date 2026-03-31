using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using Newtonsoft.Json;
using UserService.Api.AuthModels;

namespace UserService.Api.Middlewares;

public class ClaimsValidationMiddleware(RequestDelegate next)
{
    private const string AuthorizationHeaderName = "Authorization";
    private const string SchemaName = "Bearer ";

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
        try
        {
            var token = context.Request.Headers[AuthorizationHeaderName].ToString().Replace(SchemaName, string.Empty);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var payload = jsonToken.Payload.SerializeToJson();

            var claims = JsonConvert.DeserializeObject<RequiredClaims>(payload);

            return claims != null && claims.IsValid();
        }
        catch (Exception)
        {
            return false;
        }
    }
}