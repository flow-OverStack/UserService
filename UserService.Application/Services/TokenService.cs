using AutoMapper;
using UserService.Application.Exceptions.IdentityServer.Base;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services;

public class TokenService(
    IIdentityServer identityServer,
    IMapper mapper)
    : ITokenService
{
    public async Task<BaseResult<TokenDto>> RefreshTokenAsync(RefreshTokenDto dto,
        CancellationToken cancellationToken = default)
    {
        var identityResponse = await SafeRefreshTokenAsync(identityServer, dto, cancellationToken);

        if (!identityResponse.IsSuccess) return identityResponse;

        var tokenDto = mapper.Map<TokenDto>(identityResponse.Data);

        return BaseResult<TokenDto>.Success(tokenDto);
    }

    private static async Task<BaseResult<TokenDto>> SafeRefreshTokenAsync(IIdentityServer identityServer,
        RefreshTokenDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await identityServer.RefreshTokenAsync(dto, cancellationToken);
            return BaseResult<TokenDto>.Success(response);
        }
        catch (IdentityServerBusinessException e)
        {
            var baseResult = e.GetBaseResult();
            return BaseResult<TokenDto>.Failure(baseResult.ErrorMessage!, baseResult.ErrorCode);
        }
    }
}