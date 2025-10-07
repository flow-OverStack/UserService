using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Service;

namespace UserService.GraphQl.DataLoaders;

/// <summary>
///     Data loader that stores users by roles ids
/// </summary>
/// <param name="batchScheduler"></param>
/// <param name="options"></param>
public class GroupUserDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedDataLoader<long, User>(batchScheduler, options)
{
    protected override async Task<ILookup<long, User>> LoadGroupedBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var userService = scope.ServiceProvider.GetRequiredService<IGetUserService>();

        var result = await userService.GetUsersWithRolesAsync(keys, cancellationToken);

        if (!result.IsSuccess)
            return Enumerable.Empty<IGrouping<long, User>>().ToLookup(_ => 0L, _ => default(User)!); // Empty lookup

        var lookup = result.Data
            .SelectMany(x => x.Value.Select(y => new { x.Key, User = y }))
            .ToLookup(x => x.Key, x => x.User);

        return lookup;
    }
}