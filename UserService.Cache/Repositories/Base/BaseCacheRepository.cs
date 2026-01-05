using UserService.Cache.Interfaces;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository.Cache;

namespace UserService.Cache.Repositories.Base;

public class BaseCacheRepository<TEntity, TEntityId> : IBaseCacheRepository<TEntity, TEntityId>
{
    private readonly ICacheProvider _cache;
    private readonly ICacheEntityMapping<TEntity, TEntityId> _mapping;
    private readonly int _nullTimeToLiveInSeconds;
    private readonly int _timeToLiveInSeconds;

    public BaseCacheRepository(ICacheProvider cache,
        ICacheEntityMapping<TEntity, TEntityId> mapping,
        int timeToLiveInSeconds, int nullTimeToLiveInSeconds)
    {
        // We do null checks here because functions are not injected by DI and can be null.
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(mapping);

        _cache = cache;
        _mapping = mapping;
        _timeToLiveInSeconds = timeToLiveInSeconds;
        _nullTimeToLiveInSeconds = nullTimeToLiveInSeconds;
    }

    public async Task<IEnumerable<TEntity>> GetByIdsOrFetchAndCacheAsync(IEnumerable<TEntityId> ids,
        Func<IEnumerable<TEntityId>, CancellationToken, Task<IEnumerable<TEntity>>> fetch,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids);
        ArgumentNullException.ThrowIfNull(fetch);

        var idsList = ids.ToArray();

        try
        {
            var keys = idsList.Select(_mapping.GetKey).ToArray();
            var nullKeys = await _cache.GetNullKeysAsync(keys, cancellationToken);

            var aliveKeys = keys.Except(nullKeys).ToArray();
            var cached = (await _cache.GetJsonParsedAsync<TEntity>(aliveKeys, cancellationToken)).ToArray();

            var missingIds = aliveKeys.Select(_mapping.ParseIdFromKey)
                .Except(cached.Select(_mapping.GetId))
                .ToArray();

            if (missingIds.Length > 0)
                return await GetFromInnerAndCacheAsync(missingIds, cached);

            return cached;
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<IEnumerable<TEntity>> GetFromInnerAndCacheAsync(IEnumerable<TEntityId> missingIds,
            IEnumerable<TEntity> cached)
        {
            var cachedData = cached.ToArray();
            var missingIdsArray = missingIds.ToArray();

            var fetchedData = (await fetch(missingIdsArray, cancellationToken)).ToArray();

            try
            {
                var notFoundIds = missingIdsArray.Except(fetchedData.Select(_mapping.GetId)).ToArray();
                var notFoundKeys = notFoundIds.Select(_mapping.GetKey);
                await _cache.MarkAsNullAsync(notFoundKeys, _nullTimeToLiveInSeconds, true, CancellationToken.None);

                var keyValues = fetchedData.Select(x =>
                    new KeyValuePair<string, TEntity>(_mapping.GetKey(_mapping.GetId(x)), x));
                await _cache.StringSetAsync(keyValues, _timeToLiveInSeconds, true, CancellationToken.None);
            }
            catch (Exception)
            {
                // If caching fails, we still return the fetched data without caching it.
            }

            var allEntities = fetchedData.UnionBy(cachedData, _mapping.GetId).ToArray();
            return allEntities;
        }
    }

    public async Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<TEntity>>>>
        GetGroupedByOuterIdOrFetchAndCacheAsync<TOuterId>(
            IEnumerable<TOuterId> outerIds,
            Func<TOuterId, string> getOuterEntityKey,
            Func<TOuterId, string> getOuterKey,
            Func<string, TOuterId> parseOuterIdFromKey,
            Func<IEnumerable<TOuterId>, CancellationToken,
                Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<TEntity>>>>> fetch,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(outerIds);
        ArgumentNullException.ThrowIfNull(getOuterKey);
        ArgumentNullException.ThrowIfNull(parseOuterIdFromKey);
        ArgumentNullException.ThrowIfNull(fetch);

        var idsList = outerIds.ToArray();

        try
        {
            string[] outerEntityKeys = [..idsList.Select(getOuterEntityKey), ..idsList.Select(getOuterKey)];
            var nullOuterEntityKeys = await _cache.GetNullKeysAsync(outerEntityKeys, cancellationToken);
            var aliveIds = idsList.Except(nullOuterEntityKeys.Select(parseOuterIdFromKey)).ToArray();

            var outerKeys = aliveIds.Select(getOuterKey);
            var outerSets = (await _cache.SetsStringMembersAsync(outerKeys, cancellationToken))
                .Where(x => x.Value.Any());

            var outerToEntityIds = outerSets
                .Select(x => new KeyValuePair<TOuterId, IEnumerable<TEntityId>>(
                    parseOuterIdFromKey(x.Key),
                    x.Value.Select(_mapping.ParseIdFromValue)))
                .ToArray();

            var entityKeys = outerToEntityIds.SelectMany(x => x.Value.Select(_mapping.GetKey)).Distinct();
            var allEntities = (await _cache.GetJsonParsedAsync<TEntity>(entityKeys, cancellationToken)).ToArray();

            var grouped = outerToEntityIds.Select(kvp =>
                new KeyValuePair<TOuterId, IEnumerable<TEntity>>(
                    kvp.Key,
                    kvp.Value
                        .Select(id => allEntities.FirstOrDefault(e =>
                            EqualityComparer<TEntityId>.Default.Equals(_mapping.GetId(e), id)))
                        .Where(e => !Equals(e, default(TEntity)))!)).ToArray();

            var missingOuterIds = aliveIds.Except(outerToEntityIds.Select(x => x.Key)).ToList();
            var cached = new List<KeyValuePair<TOuterId, IEnumerable<TEntity>>>();

            foreach (var outerToEntityId in outerToEntityIds)
            {
                var actual = grouped.First(x => EqualityComparer<TOuterId>.Default.Equals(x.Key, outerToEntityId.Key));

                var cachedIds = actual.Value.Select(_mapping.GetId);
                if (outerToEntityId.Value.Except(cachedIds).Any())
                    missingOuterIds.Add(outerToEntityId.Key);
                else
                    cached.Add(actual);
            }

            if (missingOuterIds.Count > 0)
                return await GetFromInnerAndCacheAsync(missingOuterIds, cached);

            return grouped;
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<TEntity>>>> GetFromInnerAndCacheAsync(
            IEnumerable<TOuterId> missingIds, IEnumerable<KeyValuePair<TOuterId, IEnumerable<TEntity>>> cached)
        {
            var cachedData = cached.ToArray();
            var missingIdsArray = missingIds.ToArray();

            var fetchedData = (await fetch(missingIdsArray, cancellationToken)).ToArray();

            try
            {
                var notFoundIds = missingIdsArray.Except(fetchedData.Select(x => x.Key)).ToArray();
                var notFoundKeys = notFoundIds.Select(getOuterKey);
                await _cache.MarkAsNullAsync(notFoundKeys, _nullTimeToLiveInSeconds, true, CancellationToken.None);

                var outerSetToCache = fetchedData.Select(kvp =>
                    new KeyValuePair<string, IEnumerable<string>>(
                        getOuterKey(kvp.Key),
                        kvp.Value.Select(_mapping.GetValue)));

                var entities = fetchedData.SelectMany(x => x.Value);
                var entityToCache = entities.Select(e =>
                    new KeyValuePair<string, TEntity>(_mapping.GetKey(_mapping.GetId(e)), e));

                await _cache.StringSetAsync(entityToCache, _timeToLiveInSeconds, true, CancellationToken.None);
                await _cache.SetsAddAsync(outerSetToCache, _timeToLiveInSeconds, true, CancellationToken.None);
            }
            catch (Exception)
            {
                // If caching fails, we still return the fetched data without caching it.
            }

            var allData = fetchedData.UnionBy(cachedData, x => x.Key).ToArray();
            return allData;
        }
    }
}