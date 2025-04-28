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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Role deleting
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<RoleDto>> DeleteRoleAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Role updating
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adding role for user
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes user's role
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(DeleteUserRoleDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updating role for user
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto,
        CancellationToken cancellationToken = default);
}