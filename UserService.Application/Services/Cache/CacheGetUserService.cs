using UserService.Application.Enums;
using UserService.Application.Resources;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services.Cache;

public class CacheGetUserService(IUserCacheRepository cacheRepository, IGetUserService inner) : IGetUserService
{
    public Task<QueryableResult<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return inner.GetAllAsync(cancellationToken);
    }

    public async Task<CollectionResult<User>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var users = (await cacheRepository.GetByIdsOrFetchAndCacheAsync(idsArray,
            async (idsToFetch, ct) => (await inner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (users.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<User>.Failure(ErrorMessage.UserNotFound,
                    (int)ErrorCodes.UserNotFound),
                > 1 => CollectionResult<User>.Failure(ErrorMessage.UsersNotFound,
                    (int)ErrorCodes.UsersNotFound)
            };

        return CollectionResult<User>.Success(users);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<User>>>> GetUsersWithRolesAsync(
        IEnumerable<long> roleIds, CancellationToken cancellationToken = default)
    {
        var groupedUsers = (await cacheRepository.GetUsersWithRolesOrFetchAndCacheAsync(roleIds,
                async (idsToFetch, ct) => (await inner.GetUsersWithRolesAsync(idsToFetch, ct)).Data ?? [],
                cancellationToken))
            .ToArray();

        if (groupedUsers.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Failure(ErrorMessage.UsersNotFound,
                (int)ErrorCodes.UsersNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Success(groupedUsers);
    }

    public async Task<CollectionResult<KeyValuePair<long, int>>> GetCurrentReputationsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var reputations = (await cacheRepository.GetCurrentReputationsOrFetchAndCacheAsync(idsArray,
                async (idsToFetch, ct) => (await inner.GetCurrentReputationsAsync(idsToFetch, ct)).Data ?? [],
                cancellationToken))
            .ToArray();

        if (reputations.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<KeyValuePair<long, int>>.Failure(ErrorMessage.UserNotFound,
                    (int)ErrorCodes.UserNotFound),
                > 1 => CollectionResult<KeyValuePair<long, int>>.Failure(ErrorMessage.UsersNotFound,
                    (int)ErrorCodes.UsersNotFound)
            };

        return CollectionResult<KeyValuePair<long, int>>.Success(reputations);
    }

    public async Task<CollectionResult<KeyValuePair<long, int>>> GetRemainingReputationsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var reputations =
            (await cacheRepository.GetRemainingReputationsOrFetchAndCacheAsync(idsArray,
                async (idsToFetch, ct) => (await inner.GetRemainingReputationsAsync(idsToFetch, ct)).Data ?? [],
                cancellationToken))
            .ToArray();

        if (reputations.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<KeyValuePair<long, int>>.Failure(ErrorMessage.UserNotFound,
                    (int)ErrorCodes.UserNotFound),
                > 1 => CollectionResult<KeyValuePair<long, int>>.Failure(ErrorMessage.UsersNotFound,
                    (int)ErrorCodes.UsersNotFound)
            };

        return CollectionResult<KeyValuePair<long, int>>.Success(reputations);
    }
}