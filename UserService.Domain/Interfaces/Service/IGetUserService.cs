using UserService.Domain.Entities;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

public interface IGetUserService : IGetService<User>
{
    /// <summary>
    ///     Gets one user by its id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<User>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all users who have the roles by their ids
    /// </summary>
    /// <param name="roleIds"></param>
    /// <returns></returns>
    /// <param name="cancellationToken"></param>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<User>>>> GetUsersWithRolesAsync(IEnumerable<long> roleIds,
        CancellationToken cancellationToken = default);
}