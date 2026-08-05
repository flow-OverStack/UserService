using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;
using UserService.GraphQl.DataLoaders.Base;

namespace UserService.GraphQl.DataLoaders;

public class ReputationRuleDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : EntityBatchDataLoader<ReputationRule, long>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<ReputationRule>> FetchAsync(IServiceProvider scopedProvider,
        IReadOnlyList<long> keys, CancellationToken cancellationToken) =>
        scopedProvider.GetRequiredService<IGetReputationRuleService>().GetByIdsAsync(keys, cancellationToken);

    protected override long GetId(ReputationRule entity) => entity.Id;
}
