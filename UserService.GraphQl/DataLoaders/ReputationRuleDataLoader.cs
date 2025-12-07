using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Service;

namespace UserService.GraphQl.DataLoaders;

public class ReputationRuleDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : BatchDataLoader<long, ReputationRule>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<long, ReputationRule>> LoadBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var userService = scope.ServiceProvider.GetRequiredService<IGetReputationRuleService>();

        var result = await userService.GetByIdsAsync(keys, cancellationToken);

        var dictionary = new Dictionary<long, ReputationRule>();

        if (!result.IsSuccess)
            return dictionary.AsReadOnly();

        dictionary = result.Data.ToDictionary(x => x.Id, x => x);

        return dictionary.AsReadOnly();
    }
}