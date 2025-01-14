using UserService.Domain.Dto.Role;
using UserService.Domain.Dto.UserRole;
using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

/// <summary>
///     Service for roles' control
/// </summary>
public interface IRoleService
{
    /// <summary>
    ///     Role creating
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto);

    /// <summary>
    ///     Role deleting
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BaseResult<RoleDto>> DeleteRoleAsync(long id);

    /// <summary>
    ///     Role updating
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto);

    /// <summary>
    ///     Adding role for user
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto);

    /// <summary>
    ///     Deletes user's role
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(DeleteUserRoleDto dto);

    /// <summary>
    ///     Updating role for user
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto);
}