using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Results;

namespace UserService.Cache.Repositories;

public class BaseCacheRepository<TEntity, TEntityId>
    : IBaseCacheRepository<TEntity, TEntityId> where TEntity : class
{
    private readonly ICacheProvider _cache;
    private readonly Func<TEntity, TEntityId> _entityIdSelector;
    private readonly Func<TEntityId, string> _getEntityKey;
    private readonly Func<TEntity, string> _getEntityValue;
    private readonly Func<string, TEntityId> _parseEntityIdFromValue;

    public BaseCacheRepository(ICacheProvider cache,
        Func<TEntity, TEntityId> entityIdSelector,
        Func<TEntityId, string> getEntityKey,
        Func<TEntity, string> getEntityValue,
        Func<string, TEntityId> parseEntityIdFromValue)
    {
        // We do null checks here because functions are not injected by DI and can be null.

        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(entityIdSelector);
        ArgumentNullException.ThrowIfNull(getEntityKey);
        ArgumentNullException.ThrowIfNull(getEntityValue);
        ArgumentNullException.ThrowIfNull(parseEntityIdFromValue);

        _cache = cache;
        _entityIdSelector = entityIdSelector;
        _getEntityKey = getEntityKey;
        _getEntityValue = getEntityValue;
        _parseEntityIdFromValue = parseEntityIdFromValue;
    }

    public async Task<CollectionResult<TEntity>> GetByIdsOrFetchAndCacheAsync(
        IEnumerable<TEntityId> ids,
        Func<IEnumerable<TEntityId>, CancellationToken, Task<CollectionResult<TEntity>>> fetch,
        int timeToLiveInSeconds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ids);
        ArgumentNullException.ThrowIfNull(fetch);

        var idsList = ids.ToArray();

        try
        {
            var keys = idsList.Select(_getEntityKey);
            var cached = (await _cache.GetJsonParsedAsync<TEntity>(keys, cancellationToken)).ToArray();

            var missingIds = idsList.Except(cached.Select(_entityIdSelector)).ToArray();

            if (missingIds.Length > 0)
                return await GetFromInnerAndCacheAsync(missingIds, cached);

            return CollectionResult<TEntity>.Success(cached);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<TEntity>> GetFromInnerAndCacheAsync(IEnumerable<TEntityId> missingIds,
            IEnumerable<TEntity> alreadyCached)
        {
            var alreadyCachedData = alreadyCached.ToArray();

            var result = await fetch(missingIds, cancellationToken);
            if (!result.IsSuccess)
                return alreadyCachedData.Length > 0
                    ? CollectionResult<TEntity>.Success(alreadyCachedData)
                    : result;

            try
            {
                var keyValues = result.Data.Select(x =>
                    new KeyValuePair<string, TEntity>(_getEntityKey(_entityIdSelector(x)), x));
                await _cache.StringSetAsync(keyValues, timeToLiveInSeconds, true, CancellationToken.None);
            }
            catch (Exception)
            {
                // If caching fails, we still return the fetched data without caching it.
            }

            var allEntities = result.Data.UnionBy(alreadyCachedData, _entityIdSelector).ToArray();
            return CollectionResult<TEntity>.Success(allEntities);
        }
    }

    public async Task<CollectionResult<KeyValuePair<TOuterId, IEnumerable<TEntity>>>>
        GetGroupedByOuterIdOrFetchAndCacheAsync<TOuterId>(
            IEnumerable<TOuterId> outerIds,
            Func<TOuterId, string> getOuterKey,
            Func<string, TOuterId> parseOuterIdFromKey,
            Func<IEnumerable<TOuterId>, CancellationToken,
                Task<CollectionResult<KeyValuePair<TOuterId, IEnumerable<TEntity>>>>> fetch,
            int timeToLiveInSeconds,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(outerIds);
        ArgumentNullException.ThrowIfNull(getOuterKey);
        ArgumentNullException.ThrowIfNull(parseOuterIdFromKey);
        ArgumentNullException.ThrowIfNull(fetch);

        var idsList = outerIds.ToArray();

        try
        {
            var outerKeys = idsList.Select(getOuterKey);
            var outerSets = (await _cache.SetsStringMembersAsync(outerKeys, cancellationToken))
                .Where(x => x.Value.Any());

            var outerToEntityIds = outerSets
                .Select(x => new KeyValuePair<TOuterId, IEnumerable<TEntityId>>(
                    parseOuterIdFromKey(x.Key),
                    x.Value.Select(_parseEntityIdFromValue)))
                .ToArray();

            var entityKeys = outerToEntityIds.SelectMany(x => x.Value.Select(_getEntityKey)).Distinct();
            var allEntities = (await _cache.GetJsonParsedAsync<TEntity>(entityKeys, cancellationToken)).ToArray();

            var grouped = outerToEntityIds.Select(kvp =>
                new KeyValuePair<TOuterId, IEnumerable<TEntity>>(
                    kvp.Key,
                    kvp.Value
                        .Select(id => allEntities.FirstOrDefault(e =>
                            EqualityComparer<TEntityId>.Default.Equals(_entityIdSelector(e), id)))
                        .Where(e => e != null)!)).ToArray();

            var missingOuterIds = idsList.Except(outerToEntityIds.Select(x => x.Key)).ToList();
            var cached = new List<KeyValuePair<TOuterId, IEnumerable<TEntity>>>();

            foreach (var outerToEntityId in outerToEntityIds)
            {
                var actual = grouped.First(x => EqualityComparer<TOuterId>.Default.Equals(x.Key, outerToEntityId.Key));

                var cachedIds = actual.Value.Select(_entityIdSelector);
                if (outerToEntityId.Value.Except(cachedIds).Any())
                    missingOuterIds.Add(outerToEntityId.Key);
                else
                    cached.Add(actual);
            }

            if (missingOuterIds.Count > 0)
                return await GetFromInnerAndCacheAsync(missingOuterIds, cached);

            return CollectionResult<KeyValuePair<TOuterId, IEnumerable<TEntity>>>.Success(grouped);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<KeyValuePair<TOuterId, IEnumerable<TEntity>>>> GetFromInnerAndCacheAsync(
            IEnumerable<TOuterId> missingIds, IEnumerable<KeyValuePair<TOuterId, IEnumerable<TEntity>>> alreadyCached)
        {
            var alreadyCachedData = alreadyCached.ToArray();

            var result = await fetch(missingIds, cancellationToken);
            if (!result.IsSuccess)
                return alreadyCachedData.Length > 0
                    ? CollectionResult<KeyValuePair<TOuterId, IEnumerable<TEntity>>>.Success(alreadyCachedData)
                    : result;

            var fetchedData = result.Data.ToArray();

            try
            {
                var outerSetToCache = fetchedData.Select(kvp =>
                    new KeyValuePair<string, IEnumerable<string>>(
                        getOuterKey(kvp.Key),
                        kvp.Value.Select(_getEntityValue)));

                var entities = fetchedData.SelectMany(x => x.Value);
                var entityToCache = entities.Select(e =>
                    new KeyValuePair<string, TEntity>(_getEntityKey(_entityIdSelector(e)), e));

                await _cache.StringSetAsync(entityToCache, timeToLiveInSeconds, true, CancellationToken.None);
                await _cache.SetsAddAsync(outerSetToCache, timeToLiveInSeconds, true, CancellationToken.None);
            }
            catch (Exception)
            {
                // If caching fails, we still return the fetched data without caching it.
            }

            var allData = fetchedData.UnionBy(alreadyCachedData, x => x.Key).ToArray();
            return CollectionResult<KeyValuePair<TOuterId, IEnumerable<TEntity>>>.Success(allData);
        }
    }
}