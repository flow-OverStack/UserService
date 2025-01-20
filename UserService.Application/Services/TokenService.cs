using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Domain.Dto.Keycloak.Token;
using UserService.Domain.Dto.Token;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Resources;
using UserService.Domain.Result;

namespace UserService.Application.Services;

public class TokenService(
    IBaseRepository<User> userRepository,
    IIdentityServer identityServer,
    IMapper mapper)
    : ITokenService
{
    public async Task<BaseResult<TokenDto>> RefreshToken(RefreshTokenDto dto)
    {
        var tokenValidationParameters = await identityServer.GetTokenValidationParametersAsync();

        var claimsPrincipal = await GetPrincipalFromExpiredToken(dto.AccessToken, tokenValidationParameters);
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

    private static Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token,
        TokenValidationParameters tokenValidationParameters)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claimsPrincipal =
                tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException();

            return Task.FromResult(claimsPrincipal);
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
}