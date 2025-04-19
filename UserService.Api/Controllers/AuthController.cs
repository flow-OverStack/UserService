using Microsoft.AspNetCore.Mvc;
using UserService.Api.Controllers.Base;
using UserService.Domain.Dto.Token;
using UserService.Domain.Dto.User;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Result;

namespace UserService.Api.Controllers;

/// <summary>
///     Authentication controller
/// </summary>
/// <response code="200">If user was registered/logged in</response>
/// <response code="400">If user was not registered/logged in</response>
/// <response code="500">If internal server error occured</response>
public class AuthController(IAuthService authService) : BaseController
{
    /// <summary>
    ///     user registration
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<ActionResult<BaseResult<UserDto>>> Register([FromBody] RegisterUserDto dto)
    {
        var result = await authService.RegisterAsync(dto);

        return HandleResult(result);
    }

    /// <summary>
    ///     user login with email
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("login-email")]
    public async Task<ActionResult<BaseResult<TokenDto>>> Login([FromBody] LoginEmailUserDto dto)
    {
        var result = await authService.LoginWithEmailAsync(dto);

        return HandleResult(result);
    }

    /// <summary>
    ///     user login with username
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("login-username")]
    public async Task<ActionResult<BaseResult<TokenDto>>> Login([FromBody] LoginUsernameUserDto dto)
    {
        var result = await authService.LoginWithUsernameAsync(dto);

        return HandleResult(result);
    }
}