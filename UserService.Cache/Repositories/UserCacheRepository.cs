using Microsoft.Extensions.Options;
using UserService.Application.Services;
using UserService.Cache.Helpers;
using UserService.Cache.Repositories.Base;
using UserService.Cache.Settings;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;

namespace UserService.Cache.Repositories;

public class UserCacheRepository : IUserCacheRepository
{
    private readonly ICacheProvider _cacheProvider;
    private readonly RedisSettings _redisSettings;
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
        _cacheProvider = cacheProvider;
        _redisSettings = settings;
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

    public async Task<IEnumerable<KeyValuePair<long, int>>> GetCurrentReputationsOrFetchAndCacheAsync(
        IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsList = ids.ToArray();

        try
        {
            var keys = idsList.Select(CacheKeyHelper.GetUserCurrentReputationKey).ToArray();
            var cached = await _cacheProvider.StringGetAsync(keys, cancellationToken);

            var parsedCached = cached.Select(x =>
                    new KeyValuePair<long, int>(CacheKeyHelper.GetIdFromKey(x.Key), int.Parse(x.Value)))
                .ToList();

            var missingIds = idsList.Except(parsedCached.Select(x => x.Key)).ToArray();
            if (missingIds.Length > 0)
                return await GetFromInnerAndCacheAsync(missingIds, parsedCached);

            return parsedCached;
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<IEnumerable<KeyValuePair<long, int>>> GetFromInnerAndCacheAsync(IEnumerable<long> missingIds,
            IEnumerable<KeyValuePair<long, int>> cached)
        {
            var cachedData = cached.ToArray();

            var result = await _userInner.GetCurrentReputationsAsync(missingIds, cancellationToken);
            var fetchedData = result.IsSuccess ? result.Data.ToArray() : [];

            if (fetchedData.Length == 0)
                return cachedData.Length > 0
                    ? cachedData
                    : [];

            try
            {
                var keyValues = fetchedData.Select(x =>
                    new KeyValuePair<string, string>(CacheKeyHelper.GetUserCurrentReputationKey(x.Key),
                        x.Value.ToString()));
                await _cacheProvider.StringSetAsync(keyValues, _redisSettings.TimeToLiveInSeconds, true,
                    cancellationToken);
            }
            catch (Exception)
            {
                // If caching fails, we still return the fetched data without caching it.
            }

            var allReputations = fetchedData.UnionBy(cachedData, x => x.Key);
            return allReputations;
        }
    }

    public async Task<IEnumerable<KeyValuePair<long, int>>> GetRemainingReputationsOrFetchAndCacheAsync(
        IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsList = ids.ToArray();

        try
        {
            var keys = idsList.Select(CacheKeyHelper.GetUserRemainingReputationKey).ToArray();
            var cached = await _cacheProvider.StringGetAsync(keys, cancellationToken);

            var parsedCached = cached.Select(x =>
                    new KeyValuePair<long, int>(CacheKeyHelper.GetIdFromKey(x.Key), int.Parse(x.Value)))
                .ToList();

            var missingIds = idsList.Except(parsedCached.Select(x => x.Key)).ToArray();
            if (missingIds.Length > 0)
                return await GetFromInnerAndCacheAsync(missingIds, parsedCached);

            return parsedCached;
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<IEnumerable<KeyValuePair<long, int>>> GetFromInnerAndCacheAsync(IEnumerable<long> missingIds,
            IEnumerable<KeyValuePair<long, int>> cached)
        {
            var cachedData = cached.ToArray();

            var result = await _userInner.GetRemainingReputationsAsync(missingIds, cancellationToken);
            var fetchedData = result.IsSuccess ? result.Data.ToArray() : [];

            if (fetchedData.Length == 0)
                return cachedData.Length > 0
                    ? cachedData
                    : [];

            try
            {
                var keyValues = fetchedData.Select(x =>
                    new KeyValuePair<string, string>(CacheKeyHelper.GetUserRemainingReputationKey(x.Key),
                        x.Value.ToString()));
                await _cacheProvider.StringSetAsync(keyValues, _redisSettings.TimeToLiveInSeconds, true,
                    cancellationToken);
            }
            catch (Exception)
            {
                // If caching fails, we still return the fetched data without caching it.
            }

            var allReputations = fetchedData.UnionBy(cachedData, x => x.Key);
            return allReputations;
        }
    }
}