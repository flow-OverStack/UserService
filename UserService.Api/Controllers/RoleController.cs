using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Controllers.Base;
using UserService.Domain.Dto.Requests.UserRole;
using UserService.Domain.Dto.Role;
using UserService.Domain.Dto.UserRole;
using UserService.Domain.Enum;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Result;

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
    /// Create role
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for create role:
    /// 
    ///     POST
    ///     {
    ///         "name":"Admin"
    ///     }
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<BaseResult<RoleDto>>> Create([FromBody] CreateRoleDto dto)
    {
        var result = await roleService.CreateRoleAsync(dto);

        return HandleResult(result);
    }

    /// <summary>
    /// Deletes role by its id
    /// </summary>
    /// <param name="id">role's id</param>
    /// <returns></returns>
    /// /// <remarks>
    /// Request for deleting role:
    /// 
    ///     DELETE
    ///     {
    ///         "id":1
    ///     }
    /// </remarks>
    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResult<RoleDto>>> Delete(long id)
    {
        var result = await roleService.DeleteRoleAsync(id);

        return HandleResult(result);
    }

    /// <summary>
    /// Updates role
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for updating role:
    /// 
    ///     UPDATE
    ///     {
    ///         "id":1,
    ///         "name":"Admin"
    ///     }
    /// </remarks>
    [HttpPut]
    public async Task<ActionResult<BaseResult<RoleDto>>> Update([FromBody] RoleDto dto)
    {
        var result = await roleService.UpdateRoleAsync(dto);

        return HandleResult(result);
    }

    /// <summary>
    /// Adding role for user
    /// </summary>
    /// <param name="username"></param>
    /// <param name="requestDto"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for add role for user:
    /// 
    ///     POST {username}
    ///     {
    ///         "roleId":"1"
    ///     }
    /// </remarks>
    [HttpPost("{username}")]
    public async Task<ActionResult<BaseResult<UserRoleDto>>> AddRoleForUser(string username,
        [FromBody] RequestUserRoleDto requestDto)
    {
        var dto = new UserRoleDto
        {
            Username = username,
            RoleId = requestDto.RoleId
        };

        var result = await roleService.AddRoleForUserAsync(dto);

        return HandleResult(result);
    }

    /// <summary>
    /// Deletes role of user by username and role's id
    /// </summary>
    /// <param name="username"></param>
    /// <param name="roleId"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for deleting role:
    /// 
    ///     DELETE {username}/{roleId:long}
    /// </remarks>
    [HttpDelete("{username}/{roleId:long}")]
    public async Task<ActionResult<BaseResult<UserRoleDto>>> DeleteRoleForUser(string username, long roleId)
    {
        var dto = new DeleteUserRoleDto
        {
            Username = username,
            RoleId = roleId
        };

        var result = await roleService.DeleteRoleForUserAsync(dto);

        return HandleResult(result);
    }

    /// <summary>
    /// Updates role for user
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Request for updating user's role:
    /// 
    ///     UPDATE {username}
    ///     {
    ///         "fromRoleId":1,
    ///         "toRoleId":2
    ///     }
    /// </remarks>
    [HttpPut("{username}")]
    public async Task<ActionResult<BaseResult<UserRoleDto>>> UpdateRoleForUser(string username,
        [FromBody] RequestUpdateUserRoleDto requestDto)
    {
        var dto = new UpdateUserRoleDto
        {
            Username = username,
            FromRoleId = requestDto.FromRoleId,
            ToRoleId = requestDto.ToRoleId
        };

        var result = await roleService.UpdateRoleForUserAsync(dto);

        return HandleResult(result);
    }
}