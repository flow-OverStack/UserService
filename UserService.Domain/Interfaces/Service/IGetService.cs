using UserService.Domain.Interfaces.Entity;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

public interface IGetService<T> where T : IEntityId<long>
{
    /// <summary>
    ///     Gets all of T
    /// </summary>
    /// <returns></returns>
    /// <param name="cancellationToken"></param>
    Task<QueryableResult<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets multiple T's by their ids
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<T>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
}