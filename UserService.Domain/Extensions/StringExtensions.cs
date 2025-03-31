namespace UserService.Domain.Extensions;

public static class StringExtensions
{
    /// <summary>
    ///     Lowercases the first letter of input
    /// </summary>
    /// <param name="input"></param>
    /// <example>SomeInput -> someInput</example>
    /// <returns></returns>
    public static string LowercaseFirstLetter(this string input)
    {
        ArgumentException.ThrowIfNullOrEmpty(input);

        var output = char.ToLowerInvariant(input[0]) + input[1..];

        return output;
    }
}