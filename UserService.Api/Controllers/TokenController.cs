using Microsoft.AspNetCore.Mvc;
using UserService.Api.Controllers.Base;
using UserService.Domain.Dto.Token;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Result;

namespace UserService.Api.Controllers;

/// <summary>
///     Token controller
/// </summary>
/// <response code="200">If new access token was received</response>
/// <response code="400">If new access token was not received</response>
/// <response code="500">If internal server error occured</response>
public class TokenController(ITokenService tokenService) : BaseController
{
    /// <summary>
    ///     Refreshes user's token
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("refresh")]
    public async Task<ActionResult<BaseResult<TokenDto>>> RefreshToken([FromBody] RefreshTokenDto dto,
        CancellationToken cancellationToken)
    {
        var result = await tokenService.RefreshTokenAsync(dto, cancellationToken);

        return HandleResult(result);
    }
}