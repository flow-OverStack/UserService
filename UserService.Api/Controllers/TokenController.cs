using Microsoft.AspNetCore.Mvc;
using UserService.Api.Controllers.Base;
using UserService.Api.Extensions;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Api.Controllers;

/// <summary>
///     Token controller
/// </summary>
public class TokenController(ITokenService tokenService) : BaseController
{
    /// <summary>
    ///     Refreshes user's token
    /// </summary>
    /// <response code="200">If the token was refreshed successfully</response>
    /// <response code="400">If the token is invalid</response>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResult<TokenDto>>> RefreshToken([FromBody] RefreshTokenDto dto,
        CancellationToken cancellationToken)
    {
        var result = await tokenService.RefreshTokenAsync(dto, cancellationToken);

        return result.ToActionResult();
    }
}