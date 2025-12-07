using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Interfaces.Service;

namespace UserService.GraphQl.DataLoaders;

public class CurrentReputationDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : BatchDataLoader<long, int>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<long, int>> LoadBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var userService = scope.ServiceProvider.GetRequiredService<IGetUserService>();

        var result = await userService.GetCurrentReputationsAsync(keys, cancellationToken);

        var dictionary = new Dictionary<long, int>();

        if (!result.IsSuccess)
            return dictionary.AsReadOnly();

        dictionary = result.Data.ToDictionary(x => x.Key, x => x.Value);

        return dictionary.AsReadOnly();
    }
}