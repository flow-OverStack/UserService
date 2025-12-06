using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using UserService.Api.Controllers.Base;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Dtos.User;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Api.Controllers;

/// <summary>
///     Authentication controller
/// </summary>
/// <response code="200">If a user was registered/logged in</response>
/// <response code="400">If a user was not registered/logged in</response>
/// <response code="500">If internal server error occurred</response>
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
    [Obsolete("Use Authorization Code Flow with Proof Key for Code Exchange (PKCE) in the identity server instead.")]
    public async Task<ActionResult<BaseResult<UserDto>>> Register([FromBody] RegisterUserDto dto,
        CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(dto, cancellationToken);

        return HandleBaseResult(result, HttpStatusCode.Created);
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
    [Obsolete("Use Authorization Code Flow with Proof Key for Code Exchange (PKCE) in the identity server instead.")]
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
    [Obsolete("Use Authorization Code Flow with Proof Key for Code Exchange (PKCE) in the identity server instead.")]
    public async Task<ActionResult<BaseResult<TokenDto>>> Login([FromBody] LoginUsernameUserDto dto,
        CancellationToken cancellationToken)
    {
        var result = await authService.LoginWithUsernameAsync(dto, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Initializes user profile.
    ///     Must be called once after registration.
    ///     Frontend is responsible for guaranteed invocation of this endpoint.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request for user initialization:
    ///     POST init
    /// </remarks>
    [Authorize]
    [HttpPost("init")]
    public async Task<ActionResult<BaseResult<UserDto>>> Init(CancellationToken cancellationToken)
    {
        var (username, email, identityId) = GetIdentityClaims();

        if (username == null || email == null || identityId == null)
            return Unauthorized("Required claims are missing");

        var dto = new InitUserDto(username, email, identityId);

        var result = await authService.InitAsync(dto, cancellationToken);

        return HandleBaseResult(result, HttpStatusCode.Created);
    }

    private (string? username, string? email, string? identityId) GetIdentityClaims()
    {
        var username = User.Identity?.Name;
        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var identityId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return (username, email, identityId);
    }
}