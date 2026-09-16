using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;
using UserService.GraphQl.DataLoaders.Base;

namespace UserService.GraphQl.DataLoaders;

public class ReputationRecordDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : EntityBatchDataLoader<ReputationRecord, long>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<ReputationRecord>> FetchAsync(IServiceProvider scopedProvider,
        IReadOnlyList<long> keys, CancellationToken cancellationToken) =>
        scopedProvider.GetRequiredService<IGetReputationRecordService>().GetByIdsAsync(keys, cancellationToken);

    protected override long GetId(ReputationRecord entity) => entity.Id;
}
