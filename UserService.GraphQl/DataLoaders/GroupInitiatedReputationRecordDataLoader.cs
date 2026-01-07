using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Service;

namespace UserService.GraphQl.DataLoaders;

public class GroupInitiatedReputationRecordDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedDataLoader<long, ReputationRecord>(batchScheduler, options)
{
    protected override async Task<ILookup<long, ReputationRecord>> LoadGroupedBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var recordService = scope.ServiceProvider.GetRequiredService<IGetReputationRecordService>();

        var result = await recordService.GetUsersInitiatedRecordsAsync(keys, cancellationToken);

        if (!result.IsSuccess)
            return Enumerable.Empty<IGrouping<long, ReputationRecord>>()
                .ToLookup(_ => 0L, _ => default(ReputationRecord)!);

        var lookup = result.Data
            .SelectMany(x => x.Value.Select(y => new { x.Key, ReputationRecord = y }))
            .ToLookup(x => x.Key, x => x.ReputationRecord);

        return lookup;
    }
}