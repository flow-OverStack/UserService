using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Results;

namespace UserService.GraphQl.DataLoaders.Base;

/// <summary>
///     Batches a fetch of entities by id into a scoped-service call, keyed by <see cref="GetId" />.
/// </summary>
public abstract class EntityBatchDataLoader<TEntity, TId>(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : BatchDataLoader<TId, TEntity>(batchScheduler, options)
    where TId : notnull
{
    protected abstract Task<CollectionResult<TEntity>> FetchAsync(IServiceProvider scopedProvider,
        IReadOnlyList<TId> keys, CancellationToken cancellationToken);

    protected abstract TId GetId(TEntity entity);

    protected override async Task<IReadOnlyDictionary<TId, TEntity>> LoadBatchAsync(IReadOnlyList<TId> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var result = await FetchAsync(scope.ServiceProvider, keys, cancellationToken);

        return result.IsSuccess
            ? result.Data.ToDictionary(GetId).AsReadOnly()
            : new Dictionary<TId, TEntity>().AsReadOnly();
    }
}
