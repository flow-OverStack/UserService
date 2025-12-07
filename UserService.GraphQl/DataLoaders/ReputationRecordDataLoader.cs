using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Service;

namespace UserService.GraphQl.DataLoaders;

public class ReputationRecordDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : BatchDataLoader<long, ReputationRecord>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<long, ReputationRecord>> LoadBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var userService = scope.ServiceProvider.GetRequiredService<IGetReputationRecordService>();

        var result = await userService.GetByIdsAsync(keys, cancellationToken);

        var dictionary = new Dictionary<long, ReputationRecord>();

        if (!result.IsSuccess)
            return dictionary.AsReadOnly();

        dictionary = result.Data.ToDictionary(x => x.Id, x => x);

        return dictionary.AsReadOnly();
    }
}