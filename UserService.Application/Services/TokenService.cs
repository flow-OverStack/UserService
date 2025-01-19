using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        var claimsPrincipal = await identityServer.GetPrincipalFromExpiredTokenAsync(dto.AccessToken);
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