using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;
using UserService.GraphQl.DataLoaders.Base;

namespace UserService.GraphQl.DataLoaders;

/// <summary>
///     Data loader that stores users by roles ids
/// </summary>
public class GroupRoleUserDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedEntityDataLoader<User, long>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<KeyValuePair<long, IEnumerable<User>>>> FetchAsync(
        IServiceProvider scopedProvider, IReadOnlyList<long> keys, CancellationToken cancellationToken) =>
        scopedProvider.GetRequiredService<IGetUserService>().GetUsersWithRolesAsync(keys, cancellationToken);
}
