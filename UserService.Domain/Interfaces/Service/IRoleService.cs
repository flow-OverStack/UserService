using UserService.Domain.Dtos.Role;
using UserService.Domain.Dtos.UserRole;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

/// <summary>
///     Service for roles' control
/// </summary>
public interface IRoleService
{
    /// <summary>
    ///     Creates role
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes role
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<RoleDto>> DeleteRoleAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates role
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adds role for user
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
    Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(UserRoleDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates user's role
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto,
        CancellationToken cancellationToken = default);
}