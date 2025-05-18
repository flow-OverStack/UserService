using Microsoft.AspNetCore.Mvc;
using UserService.Api.Controllers.Base;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Dtos.User;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

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
    ///     Registers user
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for user registration:
    /// 
    ///     POST register
    ///     {
    ///         "username":"string",
    ///         "email":"string",
    ///         "password":"string"
    ///     }
    /// </remarks>
    [HttpPost("register")]
    public async Task<ActionResult<BaseResult<UserDto>>> Register([FromBody] RegisterUserDto dto,
        CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(dto, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Logs user in with email
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ///<remarks>
    /// Request for user login with email:
    /// 
    ///     POST login-email
    ///     {
    ///         "email":"string",
    ///         "password":"string"
    ///     }
    /// </remarks>
    [HttpPost("login-email")]
    public async Task<ActionResult<BaseResult<TokenDto>>> Login([FromBody] LoginEmailUserDto dto,
        CancellationToken cancellationToken)
    {
        var result = await authService.LoginWithEmailAsync(dto, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Logs user in with username
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for user login with username:
    /// 
    ///     POST login-username
    ///     {
    ///         "username":"string",
    ///         "password":"string"
    ///     }
    /// </remarks>
    [HttpPost("login-username")]
    public async Task<ActionResult<BaseResult<TokenDto>>> Login([FromBody] LoginUsernameUserDto dto,
        CancellationToken cancellationToken)
    {
        var result = await authService.LoginWithUsernameAsync(dto, cancellationToken);

        return HandleBaseResult(result);
    }
}