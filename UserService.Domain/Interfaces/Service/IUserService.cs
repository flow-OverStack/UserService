using UserService.Domain.Dtos.User;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

/// <summary>
///     Service for managing user profile operations
/// </summary>
public interface IUserService
{
    /// <summary>
    ///     Updates the username of the specified user.
    ///     Syncs the change with the identity server.
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<UserDto>> UpdateUsernameAsync(UpdateUsernameDto dto, CancellationToken cancellationToken = default);
}