using Newtonsoft.Json.Linq;
using UserService.Application.Resources;
using UserService.Cache.Helpers;
using UserService.Domain.Dtos.User;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository.Cache;

namespace UserService.Cache.Repositories;

public class UserActivityCacheRepository(ICacheProvider cache) : IUserActivityCacheRepository
{
    public async Task<IEnumerable<UserActivityDto>> GetValidActivitiesAsync(
        CancellationToken cancellationToken = default)
    {
        // Removing invalid keys
        var allActivityKeys =
            await cache.SetStringMembersAsync(CacheKeyHelper.GetUserActivitiesKey(), cancellationToken);
        var validActivityKeys = allActivityKeys.Where(IsValidKey);

        // Removing invalid values
        var activityValues =
            await cache.GetJsonParsedWithKeysAsync<UserActivityDto>(validActivityKeys, cancellationToken);

        var activities = activityValues.Select(x => x.Value with { UserId = ParseUserIdFromKey(x.Key) });

        return activities;
    }

    public async Task DeleteAllActivitiesAsync(CancellationToken cancellationToken = default)
    {
        var allActivityKeys =
            await cache.SetStringMembersAsync(CacheKeyHelper.GetUserActivitiesKey(), cancellationToken);
        var keysToDelete = allActivityKeys.Prepend(CacheKeyHelper.GetUserActivitiesKey());
        await cache.KeysDeleteAsync(keysToDelete, cancellationToken: cancellationToken);
    }

    public async Task AddActivityAsync(UserActivityDto dto, CancellationToken cancellationToken = default)
    {
        var key = CacheKeyHelper.GetUserActivityKey(dto.UserId);
        var value = GetActivityValue(dto);

        var activityKvp = new KeyValuePair<string, string>(key, value);
        var activitiesKeyKvp =
            new KeyValuePair<string, IEnumerable<string>>(CacheKeyHelper.GetUserActivitiesKey(), [key]);

        await cache.StringSetAsync(activityKvp, cancellationToken: cancellationToken);
        await cache.SetsAddAsync(activitiesKeyKvp, cancellationToken: cancellationToken);
    }

    private static string GetActivityValue(UserActivityDto dto)
    {
        var obj = JObject.FromObject(dto);
        obj.Remove(nameof(dto.UserId)); // UserId is stored in the key (SSoT)
        return obj.ToString();
    }

    private static bool IsValidKey(string key)
    {
        try
        {
            CacheKeyHelper.GetIdFromKey(key);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static long ParseUserIdFromKey(string key)
    {
        return !IsValidKey(key)
            ? throw new FormatException(ErrorMessage.InvalidCacheDataFormat)
            : CacheKeyHelper.GetIdFromKey(key);
    }
}