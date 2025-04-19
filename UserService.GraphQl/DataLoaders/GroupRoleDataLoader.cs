using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entity;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Services;

namespace UserService.GraphQl.DataLoaders;

/// <summary>
///     Data loader that stores roles by users ids
/// </summary>
/// <param name="batchScheduler"></param>
/// <param name="options"></param>
public class GroupRoleDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedDataLoader<long, Role>(batchScheduler, options)
{
    protected override async Task<ILookup<long, Role>> LoadGroupedBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var roleService = scope.ServiceProvider.GetRequiredService<IGetRoleService>();

        var result = await roleService.GetUsersRolesAsync(keys);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        var lookup = result.Data
            .SelectMany(x => x.Value.Select(y => new { x.Key, Role = y }))
            .ToLookup(x => x.Key, x => x.Role);

        return lookup;
    }
}