using UserService.Domain.Dtos.Role;
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
}
