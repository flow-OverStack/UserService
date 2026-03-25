using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Controllers.Base;
using UserService.Api.Dtos.User;
using UserService.Domain.Dtos.User;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Api.Controllers;

/// <summary>
///     User management controller
/// </summary>
[Authorize]
public class UserController(IUserService userService) : BaseController
{
    /// <summary>
    ///     Updates the username of the currently authenticated user
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request to update username:
    ///     PATCH me/username
    ///     {
    ///     "username":"string"
    ///     }
    /// </remarks>
    /// <response code="200">If the username was updated successfully</response>
    /// <response code="400">If the username is invalid</response>
    /// <response code="401">If required claims are missing</response>
    /// <response code="404">If the user was not found</response>
    /// <response code="409">If the username is already taken</response>
    [HttpPatch("me/username")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BaseResult<UserDto>>> UpdateMyUsername(
        [FromBody] RequestUpdateUsernameDto dto, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !long.TryParse(userIdClaim, out var userId))
            return Unauthorized("Required claims are missing");

        var serviceDto = new UpdateUsernameDto(userId, dto.Username);
        var result = await userService.UpdateUsernameAsync(serviceDto, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Updates the username of the specified user.
    ///     Requires Admin or Moderator role.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request to update username:
    ///     PATCH {id}/username
    ///     {
    ///     "username":"string"
    ///     }
    /// </remarks>
    /// <response code="200">If the username was updated successfully</response>
    /// <response code="400">If the username is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to do this operation</response>
    /// <response code="404">If the user was not found</response>
    /// <response code="409">If the username is already taken</response>
    [HttpPatch("{id:long}/username")]
    [Authorize(Roles = $"{nameof(Roles.Admin)},{nameof(Roles.Moderator)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BaseResult<UserDto>>> UpdateUsernameById(long id,
        [FromBody] RequestUpdateUsernameDto dto, CancellationToken cancellationToken)
    {
        var serviceDto = new UpdateUsernameDto(id, dto.Username);
        var result = await userService.UpdateUsernameAsync(serviceDto, cancellationToken);

        return HandleBaseResult(result);
    }
}