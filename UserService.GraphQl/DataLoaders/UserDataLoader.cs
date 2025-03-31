using UserService.Domain.Entity;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Services;

namespace UserService.GraphQl.DataLoaders;

public class UserDataLoader(IBatchScheduler batchScheduler, DataLoaderOptions options, IGetUserService userService)
    : BatchDataLoader<long, User>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<long, User>> LoadBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        var result = await userService.GetByIdsAsync(keys);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        var dictionary = new Dictionary<long, User>();
        result.Data.ToList().ForEach(x => dictionary.Add(x.Id, x));

        return dictionary.AsReadOnly();
    }
}