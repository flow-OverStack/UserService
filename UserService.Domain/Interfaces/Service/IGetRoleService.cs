using UserService.Domain.Entities;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

public interface IGetRoleService : IGetService<Role>
{
    /// <summary>
    ///     Gets all roles of the users by their ids
    /// </summary>
    /// <param name="userIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<Role>>>> GetUsersRolesAsync(IEnumerable<long> userIds,
        CancellationToken cancellationToken = default);
}