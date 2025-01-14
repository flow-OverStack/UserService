using Microsoft.AspNetCore.Mvc;
using UserService.Domain.Dto.Token;
using UserService.Domain.Interfaces.Services;

namespace UserService.Api.Controllers;

/// <summary>
///     Token controller
/// </summary>
/// <response code="200">If new access token was received</response>
/// <response code="400">If new access token was not received</response>
/// <response code="500">If internal server error occured</response>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class TokenController(ITokenService tokenService) : ControllerBase
{
    [HttpPost("refresh")]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var response = await tokenService.RefreshToken(dto);
        if (response.IsSuccess) return Ok(response);

        return BadRequest(response);
    }
}