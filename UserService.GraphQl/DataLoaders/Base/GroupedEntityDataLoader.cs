using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Results;

namespace UserService.GraphQl.DataLoaders.Base;

/// <summary>
///     Batches a fetch of entities grouped by an outer id into a scoped-service call.
/// </summary>
public abstract class GroupedEntityDataLoader<TEntity, TOuterId>(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedDataLoader<TOuterId, TEntity>(batchScheduler, options)
    where TOuterId : notnull
{
    protected abstract Task<CollectionResult<KeyValuePair<TOuterId, IEnumerable<TEntity>>>> FetchAsync(
        IServiceProvider scopedProvider, IReadOnlyList<TOuterId> keys, CancellationToken cancellationToken);

    protected override async Task<ILookup<TOuterId, TEntity>> LoadGroupedBatchAsync(IReadOnlyList<TOuterId> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var result = await FetchAsync(scope.ServiceProvider, keys, cancellationToken);

        if (!result.IsSuccess)
            return Array.Empty<TEntity>().ToLookup(_ => default(TOuterId)!);

        return result.Data
            .SelectMany(x => x.Value.Select(y => new { x.Key, Entity = y }))
            .ToLookup(x => x.Key, x => x.Entity);
    }
}
