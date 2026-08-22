using UserService.Domain.Interfaces.Entity;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

public interface IGetService<T> where T : IEntityId<long>
{
    /// <summary>
    ///     Gets all of T
    /// </summary>
    /// <returns></returns>
    QueryableResult<T> GetAll();

    /// <summary>
    ///     Gets multiple T's by their ids
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<T>> GetByIdsAsync(IReadOnlyCollection<long> ids,
        CancellationToken cancellationToken = default);
}