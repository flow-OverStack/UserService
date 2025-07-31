using Microsoft.Extensions.Options;
using UserService.Application.Services;
using UserService.Cache.Helpers;
using UserService.Cache.Settings;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;

namespace UserService.Cache.Repositories;

public class UserCacheRepository : IUserCacheRepository
{
    private readonly IBaseCacheRepository<User, long> _repository;
    private readonly IGetUserService _userInner;

    public UserCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings,
        GetUserService userInner)
    {
        var settings = redisSettings.Value;

        _repository = new BaseCacheRepository<User, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetUserKey,
            x => x.Id.ToString(),
            long.Parse,
            settings.TimeToLiveInSeconds
        );
        _userInner = userInner;
    }

    public Task<IEnumerable<User>> GetByIdsOrFetchAndCacheAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(ids,
            async (idsToFetch, ct) => (await _userInner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<User>>>> GetUsersWithRolesOrFetchAndCacheAsync(
        IEnumerable<long> roleIds,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(roleIds,
            CacheKeyHelper.GetRoleUsersKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _userInner.GetUsersWithRolesAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }
}