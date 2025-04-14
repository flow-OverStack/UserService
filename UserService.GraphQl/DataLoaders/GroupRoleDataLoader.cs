using UserService.Domain.Entity;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Services;

namespace UserService.GraphQl.DataLoaders;

public class GroupRoleDataLoader(IBatchScheduler batchScheduler, DataLoaderOptions options, IGetRoleService roleService)
    : GroupedDataLoader<long, Role>(batchScheduler, options)
{
    protected override async Task<ILookup<long, Role>> LoadGroupedBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        var result = await roleService.GetUsersRoles(keys);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        var lookup = result.Data
            .SelectMany(x => x.Value.Select(y => new { x.Key, Role = y }))
            .ToLookup(x => x.Key, x => x.Role);

        return lookup;
    }
}