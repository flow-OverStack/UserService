using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Controllers.Base;
using UserService.Api.Dtos.Role;
using UserService.Api.Dtos.UserRole;
using UserService.Domain.Dtos.Role;
using UserService.Domain.Dtos.UserRole;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Api.Controllers;

/// <summary>
///     Role controller
/// </summary>
[Authorize(Roles = nameof(Roles.Admin))]
public class RoleController(IRoleService roleService) : BaseController
{
    /// <summary>
    ///     Creates role
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request to create a role:
    ///
    ///     POST
    ///     {
    ///         "name":"Admin"
    ///     }
    /// </remarks>
    /// <response code="201">If the role was created successfully</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not an Admin</response>
    /// <response code="409">If a role with this name already exists</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BaseResult<RoleDto>>> Create([FromBody] CreateRoleDto dto,
        CancellationToken cancellationToken)
    {
        var result = await roleService.CreateRoleAsync(dto, cancellationToken);

        return HandleBaseResult(result, HttpStatusCode.Created);
    }

    /// <summary>
    ///     Deletes role by its id
    /// </summary>
    /// <param name="roleId">role's id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request to delete a role:
    ///
    ///     DELETE {roleId:long}
    /// </remarks>
    /// <response code="200">If the role was deleted successfully</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not an Admin or cannot delete default role</response>
    /// <response code="404">If the role was not found</response>
    [HttpDelete("{roleId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResult<RoleDto>>> Delete(long roleId, CancellationToken cancellationToken)
    {
        var result = await roleService.DeleteRoleAsync(roleId, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Updates role
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request to update a role:
    ///
    ///     PUT {roleId:long}
    ///     {
    ///         "name":"Admin"
    ///     }
    /// </remarks>
    /// <response code="200">If the role was updated successfully</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not an Admin</response>
    /// <response code="404">If the role was not found</response>
    [HttpPut("{roleId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResult<RoleDto>>> Update(long roleId, [FromBody] RequestRoleDto requestDto,
        CancellationToken cancellationToken)
    {
        var dto = new RoleDto(roleId, requestDto.Name);

        var result = await roleService.UpdateRoleAsync(dto, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Adds role for user
    /// </summary>
    /// <param name="username"></param>
    /// <param name="roleId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request to add a role to a user:
    ///
    ///     POST {username}/{roleId:long}
    /// </remarks>
    /// <response code="200">If the role was added to the user successfully</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not an Admin</response>
    /// <response code="404">If the user or role was not found</response>
    /// <response code="409">If the user already has this role</response>
    [HttpPost("{username}/{roleId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BaseResult<UserRoleDto>>> AddRoleForUser(string username, long roleId,
        CancellationToken cancellationToken)
    {
        var dto = new UserRoleDto
        {
            Username = username,
            RoleId = roleId
        };

        var result = await roleService.AddRoleForUserAsync(dto, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Deletes role of user by username and role's id
    /// </summary>
    /// <param name="username"></param>
    /// <param name="roleId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request to delete a role of a user:
    ///
    ///     DELETE {username}/{roleId:long}
    /// </remarks>
    /// <response code="200">If the role was deleted from the user successfully</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not an Admin or cannot delete default role</response>
    /// <response code="404">If the user or role was not found</response>
    [HttpDelete("{username}/{roleId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResult<UserRoleDto>>> DeleteRoleForUser(string username, long roleId,
        CancellationToken cancellationToken)
    {
        var dto = new UserRoleDto
        {
            Username = username,
            RoleId = roleId
        };

        var result = await roleService.DeleteRoleForUserAsync(dto, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Updates role of user
    /// </summary>
    /// <returns></returns>
    /// <param name="username"></param>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <remarks>
    /// Request to update a role of a user:
    ///
    ///     PUT {username}
    ///     {
    ///         "fromRoleId":1,
    ///         "toRoleId":2
    ///     }
    /// </remarks>
    /// <response code="200">If the user's role was updated successfully</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not an Admin</response>
    /// <response code="404">If the user or role was not found</response>
    /// <response code="409">If the user already has the target role</response>
    [HttpPut("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BaseResult<UserRoleDto>>> UpdateRoleForUser(string username,
        [FromBody] RequestUpdateUserRoleDto requestDto, CancellationToken cancellationToken)
    {
        var dto = new UpdateUserRoleDto
        {
            Username = username,
            FromRoleId = requestDto.FromRoleId,
            ToRoleId = requestDto.ToRoleId
        };

        var result = await roleService.UpdateRoleForUserAsync(dto, cancellationToken);

        return HandleBaseResult(result);
    }
}