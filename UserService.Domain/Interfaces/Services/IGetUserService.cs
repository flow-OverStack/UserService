using UserService.Domain.Entity;
using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface IGetUserService : IGetService<User>
{
    /// <summary>
    ///     Gets all users who have the roles by their ids
    /// </summary>
    /// <param name="roleIds"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<User>>>> GetUsersWithRolesAsync(IEnumerable<long> roleIds);
}