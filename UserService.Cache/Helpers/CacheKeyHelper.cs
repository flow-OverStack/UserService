namespace UserService.Cache.Helpers;

public static class CacheKeyHelper
{
    private const string UserKeyPattern = "user:{0}";
    private const string RoleUsersKeyPattern = "role:{0}:users";
    private const string RoleKeyPattern = "role:{0}";
    private const string UserRolesKeyPattern = "user:{0}:roles";

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

    public static long GetIdFromKey(string key)
    {
        var parts = key.Split(':');
        if (parts.Length < 2)
            throw new ArgumentException($"Invalid key format: {key}");

        return long.Parse(parts[1]);
    }
}