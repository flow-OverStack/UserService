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
    ///     Gets one T by its Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BaseResult<T>> GetByIdAsync(long id);
}