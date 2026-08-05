using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;
using UserService.GraphQl.DataLoaders.Base;

namespace UserService.GraphQl.DataLoaders;

public class GroupReputationRuleReputationRecordDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedEntityDataLoader<ReputationRecord, long>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>> FetchAsync(
        IServiceProvider scopedProvider, IReadOnlyList<long> keys, CancellationToken cancellationToken) =>
        scopedProvider.GetRequiredService<IGetReputationRecordService>()
            .GetRecordsWithReputationRulesAsync(keys, cancellationToken);
}
