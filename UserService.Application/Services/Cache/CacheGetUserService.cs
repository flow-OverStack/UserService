using UserService.Application.Enums;
using UserService.Application.Resources;
using UserService.Domain.Entities;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services.Cache;

public class CacheGetUserService(
    IBaseCacheRepository<User, long> cacheRepository,
    GetUserService inner,
    IBaseCacheRepository<Role, long> roleCacheRepository,
    GetRoleService roleInner) : IGetUserService
{
    public Task<QueryableResult<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return inner.GetAllAsync(cancellationToken);
    }

    public async Task<CollectionResult<User>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var users = (await cacheRepository.GetByIdsOrFetchAndCacheAsync(
            idsArray,
            async (idsToFetch, ct) => (await inner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken
        )).ToArray();

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

    public async Task<BaseResult<User>> GetByIdWithRolesAsync(long id, CancellationToken cancellationToken = default)
    {
        var users = await cacheRepository.GetByIdsOrFetchAndCacheAsync(
            [id],
            async (idsToFetch, ct) => (await inner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken
        );

        var user = users.SingleOrDefault();

        if (user == null)
            return BaseResult<User>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var groupedRoles = (await roleCacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            [user.Id],
            CacheKeyHelper.GetUserRolesKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await roleInner.GetUsersRolesAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken
        )).ToArray();

        if (groupedRoles.Length == 0 || !groupedRoles.Single().Value.Any())
            return BaseResult<User>.Failure(ErrorMessage.RolesNotFound, (int)ErrorCodes.RolesNotFound);

        user.Roles = groupedRoles.Single().Value.ToList();

        return BaseResult<User>.Success(user);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<User>>>> GetUsersWithRolesAsync(
        IEnumerable<long> roleIds, CancellationToken cancellationToken = default)
    {
        var groupedUsers = (await cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            roleIds,
            CacheKeyHelper.GetRoleUsersKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await inner.GetUsersWithRolesAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken
        )).ToArray();

        if (groupedUsers.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Failure(ErrorMessage.UsersNotFound,
                (int)ErrorCodes.UsersNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Success(groupedUsers);
    }
}