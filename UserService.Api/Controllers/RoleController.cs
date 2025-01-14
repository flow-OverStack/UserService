using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Domain.Dto.Role;
using UserService.Domain.Dto.UserRole;
using UserService.Domain.Entity;
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
[Consumes(MediaTypeNames.Application.Json)]
[Authorize(Roles = nameof(Roles.Admin))]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class RoleController(IRoleService roleService) : ControllerBase
{
    /// <summary>
    ///     Create role
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request for create role:
    ///     POST
    ///     {
    ///     "name":"Admin"
    ///     }
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<BaseResult<Role>>> Create([FromBody] CreateRoleDto dto)
    {
        var response = await roleService.CreateRoleAsync(dto);
        if (response.IsSuccess) return Ok(response);

        return BadRequest(response);
    }

    /// <summary>
    ///     Deletes role by its id
    /// </summary>
    /// <param name="id">role's id</param>
    /// <returns></returns>
    /// ///
    /// <remarks>
    ///     Request for deleting role:
    ///     DELETE
    ///     {
    ///     "id":1
    ///     }
    /// </remarks>
    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult<BaseResult<Role>>> Delete(long id)
    {
        var response = await roleService.DeleteRoleAsync(id);
        if (response.IsSuccess) return Ok(response);

        return BadRequest(response);
    }

    /// <summary>
    ///     Updates role
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request for updating role:
    ///     UPDATE
    ///     {
    ///     "id":1,
    ///     "name":"Admin"
    ///     }
    /// </remarks>
    [HttpPut]
    public async Task<ActionResult<BaseResult<Role>>> Update([FromBody] RoleDto dto)
    {
        var response = await roleService.UpdateRoleAsync(dto);
        if (response.IsSuccess) return Ok(response);

        return BadRequest(response);
    }

    /// <summary>
    ///     Adding role for user
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request for add role for user:
    ///     POST
    ///     {
    ///     "login":"user1",
    ///     "roleName":"Admin"
    ///     }
    /// </remarks>
    [HttpPost("add-role")]
    public async Task<ActionResult<BaseResult<Role>>> AddRoleForUser([FromBody] UserRoleDto dto)
    {
        var response = await roleService.AddRoleForUserAsync(dto);
        if (response.IsSuccess) return Ok(response);

        return BadRequest(response);
    }

    /// <summary>
    ///     Deletes role of user by user's login and role's id
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// ///
    /// <remarks>
    ///     Request for deleting role:
    ///     DELETE
    ///     {
    ///     "login":"user1",
    ///     "roleId":2
    ///     }
    /// </remarks>
    [HttpDelete("delete-role")]
    public async Task<ActionResult<BaseResult<Role>>> DeleteRoleForUser([FromBody] DeleteUserRoleDto dto)
    {
        var response = await roleService.DeleteRoleForUserAsync(dto);
        if (response.IsSuccess) return Ok(response);

        return BadRequest(response);
    }

    /// <summary>
    ///     Updates role for user
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request for updating user's role:
    ///     UPDATE
    ///     {
    ///     "Login":"user1",
    ///     "FromRoleId":1,
    ///     "ToRoleId":2
    ///     }
    /// </remarks>
    [HttpPut("update-role")]
    public async Task<ActionResult<BaseResult<Role>>> UpdateRoleForUser([FromBody] UpdateUserRoleDto dto)
    {
        var response = await roleService.UpdateRoleForUserAsync(dto);
        if (response.IsSuccess) return Ok(response);

        return BadRequest(response);
    }
}