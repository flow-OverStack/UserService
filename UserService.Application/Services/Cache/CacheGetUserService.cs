using Microsoft.Extensions.Options;
using UserService.Cache.Repositories;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Resources;
using UserService.Domain.Results;
using UserService.Domain.Settings;
using Role = UserService.Domain.Entities.Role;

namespace UserService.Application.Services.Cache;

public class CacheGetUserService : IGetUserService
{
    private readonly IBaseCacheRepository<User, long> _cacheRepository;
    private readonly IGetUserService _inner;
    private readonly RedisSettings _redisSettings;
    private readonly IBaseCacheRepository<Role, long> _roleCacheRepository;
    private readonly IGetRoleService _roleInner;

    public CacheGetUserService(GetUserService inner, GetRoleService roleInner, ICacheProvider cacheProvider,
        IOptions<RedisSettings> redisSettings)
    {
        _cacheRepository = new BaseCacheRepository<User, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetUserKey,
            x => x.Id.ToString(),
            long.Parse
        );
        _roleCacheRepository = new BaseCacheRepository<Role, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetRoleKey,
            x => x.Id.ToString(),
            long.Parse
        );
        _inner = inner;
        _roleInner = roleInner;
        _redisSettings = redisSettings.Value;
    }

    public Task<QueryableResult<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _inner.GetAllAsync(cancellationToken);
    }

    public async Task<CollectionResult<User>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var users = (await _cacheRepository.GetByIdsOrFetchAndCacheAsync(
            idsArray,
            async (idsToFetch, ct) => (await _inner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
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
        var users = await _cacheRepository.GetByIdsOrFetchAndCacheAsync(
            [id],
            async (idsToFetch, ct) => (await _inner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );

        var user = users.SingleOrDefault();

        if (user == null)
            return BaseResult<User>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var roles = (await _roleCacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            [user.Id],
            CacheKeyHelper.GetUserRolesKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _roleInner.GetUsersRolesAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        )).ToArray();

        if (roles.Length == 0)
            return BaseResult<User>.Failure(ErrorMessage.RolesNotFound, (int)ErrorCodes.RolesNotFound);

        user.Roles = roles.Single().Value.ToList();

        return BaseResult<User>.Success(user);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<User>>>> GetUsersWithRolesAsync(
        IEnumerable<long> roleIds, CancellationToken cancellationToken = default)
    {
        var groupedUsers = (await _cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            roleIds,
            CacheKeyHelper.GetRoleUsersKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _inner.GetUsersWithRolesAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        )).ToArray();

        if (groupedUsers.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Failure(ErrorMessage.UsersNotFound,
                (int)ErrorCodes.UsersNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Success(groupedUsers);
    }
}