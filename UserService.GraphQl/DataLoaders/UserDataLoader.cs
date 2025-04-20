using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Services;

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
        using var scope = scopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IGetUserService>();

        var result = await userService.GetByIdsAsync(keys);

        var dictionary = new Dictionary<long, User>();

        if (!result.IsSuccess)
            return dictionary;

        result.Data.ToList().ForEach(x => dictionary.Add(x.Id, x));

        return dictionary.AsReadOnly();
    }
}