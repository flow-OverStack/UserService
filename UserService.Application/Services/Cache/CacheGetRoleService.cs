using Microsoft.Extensions.Options;
using StackExchange.Redis;
using UserService.Domain.Extensions;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;
using UserService.Domain.Settings;
using Role = UserService.Domain.Entities.Role;

namespace UserService.Application.Services.Cache;

public class CacheGetRoleService(
    GetRoleService inner,
    IDatabase redisDatabase,
    IOptions<RedisSettings> redisSettings) : IGetRoleService
{
    private readonly RedisSettings _redisSettings = redisSettings.Value;

    public Task<QueryableResult<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return inner.GetAllAsync(cancellationToken);
    }

    public async Task<CollectionResult<Role>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsList = ids.ToList();

        try
        {
            var keys = idsList.Select(RedisKeyHelper.GetRoleKey);
            var roles = (await redisDatabase.GetJsonParsedAsync<Role>(keys, cancellationToken)).ToList();

            var missingIds = idsList.Except(roles.Select(x => x.Id)).ToList();

            if (missingIds.Count > 0) return await GetFromInnerAndCacheAsync(missingIds, roles);

            return CollectionResult<Role>.Success(roles);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<Role>> GetFromInnerAndCacheAsync(IEnumerable<long> missingIds,
            IEnumerable<Role> alreadyCached)
        {
            var alreadyCachedList = alreadyCached.ToList();

            var result = await inner.GetByIdsAsync(missingIds, cancellationToken);

            if (!result.IsSuccess)
                return alreadyCachedList.Count > 0
                    ? CollectionResult<Role>.Success(alreadyCachedList)
                    : result;

            var allRoles = result.Data.UnionBy(alreadyCachedList, x => x.Id).ToList();

            var keyRoles = allRoles.Select(x =>
                new KeyValuePair<string, Role>(RedisKeyHelper.GetRoleKey(x.Id), x));

            await redisDatabase.StringSetAsync(keyRoles, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);

            return CollectionResult<Role>.Success(allRoles);
        }
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Role>>>> GetUsersRolesAsync(
        IEnumerable<long> userIds,
        CancellationToken cancellationToken = default)
    {
        var idsList = userIds.ToList();

        try
        {
            var userRoleKeys = idsList.Select(RedisKeyHelper.GetUserRolesKey);
            var userRoleStringIds =
                (await redisDatabase.SetsStringMembersAsync(userRoleKeys, cancellationToken)).Where(x =>
                    x.Value.Any());

            var userRoleIds = userRoleStringIds.Select(x =>
                new KeyValuePair<long, IEnumerable<long>>(RedisKeyHelper.GetIdFromKey(x.Key),
                    x.Value.Select(long.Parse))).ToList();

            var roleKeys = userRoleIds.SelectMany(x => x.Value.Select(RedisKeyHelper.GetRoleKey))
                .Distinct();
            var roles = await redisDatabase.GetJsonParsedAsync<Role>(roleKeys, cancellationToken);

            var userRoles = userRoleIds.Select(kvp =>
                    new KeyValuePair<long, IEnumerable<Role>>(
                        kvp.Key,
                        kvp.Value
                            .Select(v => roles.FirstOrDefault(r => r.Id == v))
                            .Where(r => r != null)!))
                .ToList();

            var missingUserRoles = idsList.Except(userRoleIds.Select(x => x.Key)).Distinct().ToList();
            var cachedUserRoles = new List<KeyValuePair<long, IEnumerable<Role>>>();
            foreach (var userRoleId in userRoleIds)
            {
                // Keys in userRoleIds are guaranteed to be in userRoles

                var actualUserRole = userRoles.First(x => x.Key == userRoleId.Key);

                if (userRoleId.Value.Except(actualUserRole.Value.Select(x => x.Id)).Any())
                    missingUserRoles.Add(userRoleId.Key);
                else
                    cachedUserRoles.Add(actualUserRole);
            }

            if (missingUserRoles.Count > 0)
                return await GetFromInnerAndCacheAsync(missingUserRoles, cachedUserRoles);

            return CollectionResult<KeyValuePair<long, IEnumerable<Role>>>.Success(userRoles);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<KeyValuePair<long, IEnumerable<Role>>>> GetFromInnerAndCacheAsync(
            IEnumerable<long> missingIds, IEnumerable<KeyValuePair<long, IEnumerable<Role>>> alreadyCached)
        {
            var alreadyCachedList = alreadyCached.ToList();

            var result = await inner.GetUsersRolesAsync(missingIds, cancellationToken);

            if (!result.IsSuccess)
                return alreadyCachedList.Count > 0
                    ? CollectionResult<KeyValuePair<long, IEnumerable<Role>>>.Success(alreadyCachedList)
                    : result;

            var allUserRoles = result.Data.UnionBy(alreadyCachedList, x => x.Key).ToList();

            var userRoleStringIds = allUserRoles.Select(kvp =>
                new KeyValuePair<string, IEnumerable<string>>(RedisKeyHelper.GetUserRolesKey(kvp.Key),
                    kvp.Value.Select(x => x.Id.ToString())));

            var roles = allUserRoles.SelectMany(x => x.Value);
            var roleKeys = roles.Select(x =>
                new KeyValuePair<string, Role>(RedisKeyHelper.GetRoleKey(x.Id), x));

            await redisDatabase.StringSetAsync(roleKeys, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);
            await redisDatabase.SetsAddAsync(userRoleStringIds, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);

            return CollectionResult<KeyValuePair<long, IEnumerable<Role>>>.Success(allUserRoles);
        }
    }
}