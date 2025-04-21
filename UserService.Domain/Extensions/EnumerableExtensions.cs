namespace UserService.Domain.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    ///     Checks if collection is null or has no elements
    /// </summary>
    /// <param name="collection"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? collection)
    {
        return collection == null || !collection.Any();
    }
}