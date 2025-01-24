using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace UserService.Tests.Extensions;

internal static class SigningKeyExtensions
{
    private const string Audience = "TestAudience";
    private const string Issuer = "TestIssuer";

    static SigningKeyExtensions()
    {
        var rsa = RSA.Create();
        PrivateKey = new RsaSecurityKey(rsa);
        PublicKey = new RsaSecurityKey(rsa.ExportParameters(false));
    }

    private static RsaSecurityKey PublicKey { get; }

    private static RsaSecurityKey PrivateKey { get; }

    public static RsaSecurityKey GetPublicSigningKey()
    {
        return PublicKey;
    }

    public static string GetAudience()
    {
        return Audience;
    }

    public static string GetIssuer()
    {
        return Issuer;
    }

    public static string GetRsaToken(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim(ClaimTypes.Name, username)]),
            //expired is not listed because it is not validated
            SigningCredentials = new SigningCredentials(PrivateKey, SecurityAlgorithms.RsaSha256),
            Audience = Audience,
            Issuer = Issuer
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }

    public static string GetHmacToken(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim(ClaimTypes.Name, username)]),
            //expired is not listed because it is not validated
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey("TestSecretKeyTestSecretKeyTestSecretKey"u8.ToArray()),
                    SecurityAlgorithms.HmacSha256),
            Audience = Audience,
            Issuer = Issuer
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }
}