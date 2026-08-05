using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;
using UserService.GraphQl.DataLoaders.Base;

namespace UserService.GraphQl.DataLoaders;

public class RoleDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : EntityBatchDataLoader<Role, long>(batchScheduler, options, scopeFactory)
{
    protected override Task<CollectionResult<Role>> FetchAsync(IServiceProvider scopedProvider,
        IReadOnlyList<long> keys, CancellationToken cancellationToken) =>
        scopedProvider.GetRequiredService<IGetRoleService>().GetByIdsAsync(keys, cancellationToken);

    protected override long GetId(Role entity) => entity.Id;
}
