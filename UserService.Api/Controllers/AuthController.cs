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
    /// <response code="201">If a user was registered successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="404">If a resource was not found</response>
    /// <response code="409">If a user with this email or username already exists</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
    /// <response code="200">If a user was logged in successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the password is incorrect</response>
    /// <response code="404">If a user with this email was not found</response>
    [HttpPost("login-email")]
    [Obsolete("Use Authorization Code Flow with Proof Key for Code Exchange (PKCE) in the identity server instead.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// <response code="200">If a user was logged in successfully</response>
    /// <response code="401">If the password is incorrect</response>
    /// <response code="404">If a user with this username was not found</response>
    [HttpPost("login-username")]
    [Obsolete("Use Authorization Code Flow with Proof Key for Code Exchange (PKCE) in the identity server instead.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// <response code="200">If a user was initialized successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If required claims are missing</response>
    /// <response code="404">If a role was not found</response>
    [Authorize]
    [HttpPost("init")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResult<UserDto>>> Init(CancellationToken cancellationToken)
    {
        var (username, email, identityId) = GetIdentityClaims();

        if (username == null || email == null || identityId == null)
            return Unauthorized("Required claims are missing");

        var dto = new InitUserDto(username, email, identityId);

        var result = await authService.InitAsync(dto, cancellationToken);

        return HandleBaseResult(result);
    }

    private (string? username, string? email, string? identityId) GetIdentityClaims()
    {
        var username = User.Identity?.Name;
        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var identityId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return (username, email, identityId);
    }
}