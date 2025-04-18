using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entity;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Services;

namespace UserService.GraphQl.DataLoaders;

/// <summary>
///     Data loader that stores Users by Roles ids
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
        using var scope = scopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IGetUserService>();

        var result = await userService.GetUsersWithRoles(keys);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        var lookup = result.Data
            .SelectMany(x => x.Value.Select(y => new { x.Key, User = y }))
            .ToLookup(x => x.Key, x => x.User);

        return lookup;
    }
}