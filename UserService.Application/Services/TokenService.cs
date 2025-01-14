using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using UserService.Domain.Dto.Keycloak.Token;
using UserService.Domain.Dto.Token;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Resources;
using UserService.Domain.Result;
using UserService.Domain.Settings;

namespace UserService.Application.Services;

public class TokenService : ITokenService
{
    private readonly IIdentityServer _identityServer;
    private readonly KeycloakSettings _keycloakSettings;
    private readonly IBaseRepository<User> _userRepository;

    public TokenService(IBaseRepository<User> userRepository, IOptions<KeycloakSettings> keycloakSettings,
        IIdentityServer identityServer)
    {
        _userRepository = userRepository;
        _identityServer = identityServer;
        _keycloakSettings = keycloakSettings.Value;
    }

    public async Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string accessToken)
    {
        var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            _keycloakSettings.MetadataAddress,
            new OpenIdConnectConfigurationRetriever());

        var openIdConfiguration = await configurationManager.GetConfigurationAsync();


        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = openIdConfiguration.SigningKeys,
            ValidateLifetime = false,
            ValidAudience = _keycloakSettings.Audience,
            ValidIssuer = openIdConfiguration.Issuer
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var claimsPrincipal =
                tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase)) //Check what is jwtSecurityToken
                throw new SecurityTokenException();

            return claimsPrincipal;
        }
        catch (SecurityTokenException)
        {
            throw new SecurityTokenException(ErrorMessage.InvalidToken);
        }
        catch (Exception exception)
        {
            throw new SecurityTokenException(ErrorMessage.InvalidToken + exception.Message);
        }
    }

    public async Task<BaseResult<TokenDto>> RefreshToken(RefreshTokenDto dto)
    {
        var claimsPrincipal = await GetPrincipalFromExpiredToken(dto.AccessToken);
        var username = claimsPrincipal.Identity?.Name;

        var user = await _userRepository.GetAll()
            .Include(x => x.UserToken)
            .FirstOrDefaultAsync(x => x.Username == username);

        if (user == null || user.UserToken.RefreshToken != dto.RefreshToken ||
            user.UserToken.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return new BaseResult<TokenDto>
            {
                ErrorMessage = ErrorMessage.InvalidClientRequest
            };


        var keycloakResponse = await _identityServer.RefreshTokenAsync(new KeycloakRefreshTokenDto(dto.RefreshToken));

        var newAccessToken = keycloakResponse.AccessToken;
        var newRefreshToken = keycloakResponse.RefreshToken;

        user.UserToken.RefreshToken = newRefreshToken;
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return new BaseResult<TokenDto>
        {
            Data = new TokenDto
            {
                RefreshToken = newRefreshToken,
                AccessToken = newAccessToken,
                Expires = keycloakResponse.Expires,
                UserId = user.Id
            }
        };
    }
}