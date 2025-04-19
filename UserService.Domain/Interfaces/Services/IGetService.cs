using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface IGetService<T>
{
    /// <summary>
    ///     Gets all of T
    /// </summary>
    /// <returns></returns>
    Task<CollectionResult<T>> GetAllAsync();

    /// <summary>
    ///     Gets one T by its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BaseResult<T>> GetByIdAsync(long id);

    /// <summary>
    ///     Gets multiple T's by their ids
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<CollectionResult<T>> GetByIdsAsync(IEnumerable<long> ids);
}