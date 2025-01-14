using Microsoft.AspNetCore.Mvc;
using UserService.Domain.Dto.Token;
using UserService.Domain.Dto.User;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Result;

namespace UserService.Api.Controllers;

/// <summary>
///     authentication controller
/// </summary>
/// \
/// <response code="200">If user was registered/logged in</response>
/// <response code="400">If user was not registered/logged in</response>
/// <response code="500">If internal server error occured</response>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    ///     user registration
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<ActionResult<BaseResult>> Register([FromBody] RegisterUserDto dto)
    {
        var response = await authService.Register(dto);
        if (response.IsSuccess) return Ok(response);

        return BadRequest(response);
    }

    /// <summary>
    ///     user login with email
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("login-email")]
    public async Task<ActionResult<BaseResult<TokenDto>>> Login([FromBody] LoginEmailUserDto dto)
    {
        var response = await authService.LoginWithEmail(dto);
        if (response.IsSuccess) return Ok(response);

        return BadRequest(response);
    }

    /// <summary>
    ///     user login with username
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("login-username")]
    public async Task<ActionResult<BaseResult<TokenDto>>> Login([FromBody] LoginUsernameUserDto dto)
    {
        var response = await authService.LoginWithUsername(dto);
        if (response.IsSuccess) return Ok(response);

        return BadRequest(response);
    }
}