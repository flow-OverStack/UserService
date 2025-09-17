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
/// <response code="200">If role was created/deleted/updated/received/added</response>
/// <response code="400">If role was not created/deleted/updated/received/added</response>
/// <response code="500">If internal server error occured</response>
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
    [HttpPost]
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
    [HttpDelete("{roleId:long}")]
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
    [HttpPut("{roleId:long}")]
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
    [HttpPost("{username}/{roleId:long}")]
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
    [HttpDelete("{username}/{roleId:long}")]
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
    [HttpPut("{username}")]
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