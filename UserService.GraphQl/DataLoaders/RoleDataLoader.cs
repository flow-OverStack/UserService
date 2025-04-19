using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Entity;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Services;

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
        using var scope = scopeFactory.CreateScope();
        var roleService = scope.ServiceProvider.GetRequiredService<IGetRoleService>();

        var result = await roleService.GetByIdsAsync(keys);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        var dictionary = new Dictionary<long, Role>();
        result.Data.ToList().ForEach(x => dictionary.Add(x.Id, x));

        return dictionary.AsReadOnly();
    }
}