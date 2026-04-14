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
    ///     Logs user in (and initializes in the DB if needed) with email or username
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for user login (and DB initialization if needed):
    ///
    ///     POST login
    ///     {
    ///         "identifier":"string (email or username)",
    ///         "password":"string"
    ///     }
    /// </remarks>
    /// <response code="200">If a user was logged in successfully</response>
    /// <response code="401">If the password is incorrect</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResult<TokenDto>>> Login([FromBody] LoginUserDto dto,
        CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(dto, cancellationToken);

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
        var username = User.Identity!.Name!;
        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email)!;
        var identityId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;

        var dto = new InitUserDto(username, email, identityId);

        var result = await authService.InitAsync(dto, cancellationToken);

        return HandleBaseResult(result);
    }
}