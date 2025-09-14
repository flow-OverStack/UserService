using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Service;

namespace UserService.GraphQl.DataLoaders;

public class RoleDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : BatchDataLoader<long, Role>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<long, Role>> LoadBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var roleService = scope.ServiceProvider.GetRequiredService<IGetRoleService>();

        var result = await roleService.GetByIdsAsync(keys, cancellationToken);

        var dictionary = new Dictionary<long, Role>();

        if (!result.IsSuccess)
            return dictionary.AsReadOnly();

        dictionary = result.Data.ToDictionary(x => x.Id, x => x);

        return dictionary.AsReadOnly();
    }
}