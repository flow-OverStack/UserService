using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
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

public class TokenService(
    IBaseRepository<User> userRepository,
    IOptions<KeycloakSettings> keycloakSettings,
    IIdentityServer identityServer,
    IMapper mapper)
    : ITokenService
{
    private readonly KeycloakSettings _keycloakSettings = keycloakSettings.Value;

    public async Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string accessToken)
    {
        var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            _keycloakSettings.MetadataAddress,
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever
            {
                RequireHttps = false
            });

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
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256,
                    StringComparison.InvariantCultureIgnoreCase))
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

        var user = await userRepository.GetAll()
            .Include(x => x.UserToken)
            .FirstOrDefaultAsync(x => x.Username == username);

        if (user == null || user.UserToken.RefreshToken != dto.RefreshToken ||
            user.UserToken.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return new BaseResult<TokenDto>
            {
                ErrorMessage = ErrorMessage.InvalidClientRequest
            };


        var keycloakResponse = await identityServer.RefreshTokenAsync(new KeycloakRefreshTokenDto(dto.RefreshToken));

        user.UserToken.RefreshToken = keycloakResponse.RefreshToken;
        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        var tokenDto = mapper.Map<TokenDto>(keycloakResponse);
        tokenDto.UserId = user.Id;

        return new BaseResult<TokenDto>
        {
            Data = tokenDto
        };
    }
}