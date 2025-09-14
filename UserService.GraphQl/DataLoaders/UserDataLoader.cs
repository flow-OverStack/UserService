using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Service;

namespace UserService.GraphQl.DataLoaders;

public class UserDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : BatchDataLoader<long, User>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<long, User>> LoadBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var userService = scope.ServiceProvider.GetRequiredService<IGetUserService>();

        var result = await userService.GetByIdsAsync(keys, cancellationToken);

        var dictionary = new Dictionary<long, User>();

        if (!result.IsSuccess)
            return dictionary;

        dictionary = result.Data.ToDictionary(x => x.Id, x => x);

        return dictionary.AsReadOnly();
    }
}