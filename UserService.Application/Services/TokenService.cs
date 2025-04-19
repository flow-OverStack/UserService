using AutoMapper;
using UserService.Domain.Dto.Token;
using UserService.Domain.Exceptions.IdentityServer.Base;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Result;

namespace UserService.Application.Services;

public class TokenService(
    IIdentityServer identityServer,
    IMapper mapper)
    : ITokenService
{
    public async Task<BaseResult<TokenDto>> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var keycloakResponse = await SafeRefreshTokenAsync(identityServer, dto);

        if (!keycloakResponse.IsSuccess) return keycloakResponse;

        var tokenDto = mapper.Map<TokenDto>(keycloakResponse.Data);

        return BaseResult<TokenDto>.Success(tokenDto);
    }

    private static async Task<BaseResult<TokenDto>> SafeRefreshTokenAsync(IIdentityServer identityServer,
        RefreshTokenDto dto)
    {
        try
        {
            var response = await identityServer.RefreshTokenAsync(dto);
            return BaseResult<TokenDto>.Success(response);
        }
        catch (IdentityServerBusinessException e)
        {
            var baseResult = e.GetBaseResult();
            return BaseResult<TokenDto>.Failure(baseResult.ErrorMessage!, baseResult.ErrorCode);
        }
    }
}