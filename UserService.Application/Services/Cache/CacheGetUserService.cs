using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using UserService.Domain.Entities;
using UserService.Domain.Extensions;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;
using UserService.Domain.Settings;
using Role = UserService.Domain.Entities.Role;

namespace UserService.Application.Services.Cache;

public class CacheGetUserService(
    GetUserService inner,
    IDatabase redisDatabase,
    IOptions<RedisSettings> redisSettings) : IGetUserService
{
    private readonly RedisSettings _redisSettings = redisSettings.Value;

    public Task<QueryableResult<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return inner.GetAllAsync(cancellationToken);
    }

    public async Task<CollectionResult<User>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsList = ids.ToList();

        try
        {
            var keys = idsList.Select(RedisKeyHelper.GetUserKey);
            var users = (await redisDatabase.GetJsonParsedAsync<User>(keys, cancellationToken)).ToList();

            var missingIds = idsList.Except(users.Select(x => x.Id)).ToList();

            if (missingIds.Count > 0) return await GetFromInnerAndCacheAsync(missingIds, users);

            return CollectionResult<User>.Success(users);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<User>> GetFromInnerAndCacheAsync(IEnumerable<long> missingIds,
            IEnumerable<User> alreadyCached)
        {
            var alreadyCachedList = alreadyCached.ToList();

            var result = await inner.GetByIdsAsync(missingIds, cancellationToken);

            if (!result.IsSuccess)
                return alreadyCachedList.Count > 0
                    ? CollectionResult<User>.Success(alreadyCachedList)
                    : result;

            var allUsers = result.Data.UnionBy(alreadyCachedList, x => x.Id).ToList();

            var keyUsers = allUsers.Select(x =>
                new KeyValuePair<string, User>(RedisKeyHelper.GetUserKey(x.Id), x));

            await redisDatabase.StringSetAsync(keyUsers, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);

            return CollectionResult<User>.Success(allUsers);
        }
    }

    public async Task<BaseResult<User>> GetByIdWithRolesAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var userKey = RedisKeyHelper.GetUserKey(id);
            var user = await redisDatabase.GetJsonParsedOrDefaultAsync<User>(userKey, cancellationToken);

            if (user == null) return await GetFromInnerAndCacheAsync(id);

            var userRolesKey = RedisKeyHelper.GetUserRolesKey(id);
            var userRoles =
                (await redisDatabase.SetStringMembersAsync(userRolesKey, cancellationToken)).Select(long.Parse);

            var roleKeys = userRoles.Select(RedisKeyHelper.GetRoleKey);
            var roles = (await redisDatabase.GetJsonParsedAsync<Role>(roleKeys, cancellationToken)).ToList();

            if (roles.Count == 0)
                return await GetFromInnerAndCacheAsync(id);

            user.Roles = roles.ToList();

            return BaseResult<User>.Success(user);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(id);
        }

        async Task<BaseResult<User>> GetFromInnerAndCacheAsync(long missingId)
        {
            var result = await inner.GetByIdWithRolesAsync(missingId, cancellationToken);

            if (!result.IsSuccess) return result;

            var user = result.Data;
            var roles = user.Roles;

            user.Roles = null!;
            var userKey = (RedisKey)RedisKeyHelper.GetUserKey(user.Id);
            var userValue = JsonConvert.SerializeObject(user);

            roles.ForEach(x => x.Users = null!);
            var userRoleStringId =
                new KeyValuePair<string, IEnumerable<string>>(RedisKeyHelper.GetUserRolesKey(user.Id),
                    roles.Select(x => x.Id.ToString()));

            var keyRoles = roles.Select(x =>
                new KeyValuePair<string, Role>(RedisKeyHelper.GetRoleKey(x.Id), x));

            await redisDatabase.StringSetAsync(userKey, userValue,
                TimeSpan.FromSeconds(_redisSettings.TimeToLiveInSeconds));
            await redisDatabase.SetsAddAsync(userRoleStringId, _redisSettings.TimeToLiveInSeconds, cancellationToken);
            await redisDatabase.StringSetAsync(keyRoles, _redisSettings.TimeToLiveInSeconds, cancellationToken);

            return BaseResult<User>.Success(user);
        }
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<User>>>> GetUsersWithRolesAsync(
        IEnumerable<long> roleIds, CancellationToken cancellationToken = default)
    {
        var idsList = roleIds.ToList();

        try
        {
            var roleUserKeys = idsList.Select(RedisKeyHelper.GetRoleUsersKey);
            var roleUserStringIds =
                (await redisDatabase.SetsStringMembersAsync(roleUserKeys, cancellationToken)).Where(x =>
                    x.Value.Any());

            var roleUsersIds = roleUserStringIds.Select(x =>
                new KeyValuePair<long, IEnumerable<long>>(RedisKeyHelper.GetIdFromKey(x.Key),
                    x.Value.Select(long.Parse))).ToList();

            var userKeys = roleUsersIds.SelectMany(x => x.Value.Select(RedisKeyHelper.GetUserKey))
                .Distinct();
            var users = await redisDatabase.GetJsonParsedAsync<User>(userKeys, cancellationToken);

            var roleUsers = roleUsersIds.Select(kvp =>
                    new KeyValuePair<long, IEnumerable<User>>(
                        kvp.Key,
                        kvp.Value
                            .Select(v => users.FirstOrDefault(u => u.Id == v))
                            .Where(u => u != null)!))
                .ToList();

            var missingRoleUsers = idsList.Except(roleUsersIds.Select(x => x.Key)).Distinct().ToList();
            var cachedRoleUsers = new List<KeyValuePair<long, IEnumerable<User>>>();
            foreach (var roleUserId in roleUsersIds)
            {
                // Keys in roleUsersIds are guaranteed to be in roleUsers

                var actualRoleUser = roleUsers.First(x => x.Key == roleUserId.Key);

                if (roleUserId.Value.Except(actualRoleUser.Value.Select(x => x.Id)).Any())
                    missingRoleUsers.Add(roleUserId.Key);
                else
                    cachedRoleUsers.Add(actualRoleUser);
            }

            if (missingRoleUsers.Count > 0)
                return await GetFromInnerAndCacheAsync(missingRoleUsers, cachedRoleUsers);

            return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Success(roleUsers);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<KeyValuePair<long, IEnumerable<User>>>> GetFromInnerAndCacheAsync(
            IEnumerable<long> missingIds, IEnumerable<KeyValuePair<long, IEnumerable<User>>> alreadyCached)
        {
            var alreadyCachedList = alreadyCached.ToList();

            var result = await inner.GetUsersWithRolesAsync(missingIds, cancellationToken);

            if (!result.IsSuccess)
                return alreadyCachedList.Count > 0
                    ? CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Success(alreadyCachedList)
                    : result;

            var allRoleUsers = result.Data.UnionBy(alreadyCachedList, x => x.Key).ToList();

            var roleUserStringIds = allRoleUsers.Select(kvp =>
                new KeyValuePair<string, IEnumerable<string>>(RedisKeyHelper.GetRoleUsersKey(kvp.Key),
                    kvp.Value.Select(x => x.Id.ToString())));

            var users = allRoleUsers.SelectMany(x => x.Value);
            var userKeys = users.Select(x =>
                new KeyValuePair<string, User>(RedisKeyHelper.GetUserKey(x.Id), x));

            await redisDatabase.StringSetAsync(userKeys, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);
            await redisDatabase.SetsAddAsync(roleUserStringIds, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);

            return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Success(allRoleUsers);
        }
    }
}