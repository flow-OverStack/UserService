namespace UserService.Cache.Helpers;

public static class CacheKeyHelper
{
    private const string UserKeyPattern = "user:{0}";
    private const string UserRolesKeyPattern = "user:{0}:roles";

    private const string RoleUsersKeyPattern = "role:{0}:users";
    private const string RoleKeyPattern = "role:{0}";

    private const string UserActivitiesKey = "activity:users";
    private const string UserActivityKeyPattern = "activity:user:{0}";

    public static string GetUserKey(long id)
    {
        return string.Format(UserKeyPattern, id);
    }

    public static string GetRoleUsersKey(long roleId)
    {
        return string.Format(RoleUsersKeyPattern, roleId);
    }

    public static string GetRoleKey(long id)
    {
        return string.Format(RoleKeyPattern, id);
    }

    public static string GetUserRolesKey(long userId)
    {
        return string.Format(UserRolesKeyPattern, userId);
    }

    public static string GetUserActivityKey(long userId)
    {
        return string.Format(UserActivityKeyPattern, userId);
    }

    public static string GetUserActivitiesKey()
    {
        return UserActivitiesKey;
    }

    public static long GetIdFromKey(string key)
    {
        var parts = key.Split(':');

        var ex = new ArgumentException($"Invalid key format: {key}");
        return parts.Length switch
        {
            2 => long.Parse(parts[1]),
            3 => TryParseLong(parts[1]) ??
                 TryParseLong(parts[2]) ?? throw ex,
            _ => throw ex
        };
    }

    private static long? TryParseLong(string str)
    {
        return long.TryParse(str, out var result) ? result : null;
    }
}