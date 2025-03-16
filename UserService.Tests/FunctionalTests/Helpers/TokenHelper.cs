using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using UserService.Domain.Entity;

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
        var claims = new Dictionary<string, object>
        {
            { ClaimTypes.Role, roles.Select(x => x.Name).ToArray() }
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim(ClaimTypes.Name, username)]),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(PrivateKey, SecurityAlgorithms.RsaSha256),
            Audience = Audience,
            Issuer = Issuer,
            Claims = claims
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}