using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using UserService.Domain.Entities;

namespace UserService.Tests.FunctionalTests.Helpers;

internal static class TokenHelper
{
    private const string Audience = "TestAudience";
    private const string Issuer = "TestIssuer";
    private const string Kid = "test-key-id";

    private static readonly RsaSecurityKey PrivateKey;

    private static readonly string PublicJwk;

    static TokenHelper()
    {
        var rsa = RSA.Create();
        PrivateKey = new RsaSecurityKey(rsa);
        var publicKey = new RsaSecurityKey(rsa.ExportParameters(false));

        PrivateKey.KeyId = Kid;

        var rsaParams = publicKey.Parameters;

        var modulus = Base64UrlEncode(rsaParams.Modulus!);
        var exponent = Base64UrlEncode(rsaParams.Exponent!);

        var jwks = new
        {
            keys = new[]
            {
                new
                {
                    kty = "RSA",
                    use = "sig",
                    alg = "RS256",
                    kid = Kid,
                    n = modulus,
                    e = exponent
                }
            }
        };

        PublicJwk = JsonConvert.SerializeObject(jwks);
    }

    public static string GetJwk()
    {
        return PublicJwk;
    }

    public static string GetAudience()
    {
        return Audience;
    }

    public static string GetIssuer()
    {
        return Issuer;
    }

    public static string GetRsaTokenWithRoleClaims(string username, IEnumerable<Role> roles)
    {
        var claims = roles
            .Select(r => new Claim(ClaimTypes.Role, r.Name))
            .Prepend(new Claim(JwtRegisteredClaimNames.PreferredUsername, username));

        var token = claims.GetRsaTokenFromClaims();
        return token;
    }

    public static string GetRsaTokenWithIdentityData(string? username = null, string? email = null,
        string? identityId = null)
    {
        var claims = new List<Claim>();

        if (username != null)
            claims.Add(new Claim(JwtRegisteredClaimNames.PreferredUsername, username));

        if (email != null)
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, email));

        if (identityId != null)
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, identityId));

        var token = claims.GetRsaTokenFromClaims();
        return token;
    }

    private static string GetRsaTokenFromClaims(this IEnumerable<Claim> claims)
    {
        var header = new JwtHeader(new SigningCredentials(PrivateKey, SecurityAlgorithms.RsaSha256));
        var payload = new JwtPayload(
            Issuer,
            Audience,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(15)
        );

        var token = new JwtSecurityToken(header, payload);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}