using UserService.Domain.Dtos.UserRole;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

/// <summary>
///     Service for user&lt;-&gt;role assignment
/// </summary>
public interface IUserRoleService
{
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
