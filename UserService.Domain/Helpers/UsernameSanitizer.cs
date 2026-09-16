using System.Text.RegularExpressions;
using UserService.Domain.Settings;

namespace UserService.Domain.Helpers;

/// <summary>
///     Pure username-sanitization rule - no I/O, always produces the same output for the same input.
/// </summary>
public static partial class UsernameSanitizer
{
    public static string Sanitize(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return string.Empty;

        var lower = raw.ToLowerInvariant();
        var mapped = new string(lower.Select(c => IsAllowedChar(c) ? c : '_').ToArray());
        var collapsed = UsernameRegex().Replace(mapped, "_");
        var trimmed = collapsed.Trim('_', '-', '.');

        return trimmed.Length > EntityConstraints.UsernameMaxLength
            ? trimmed[..EntityConstraints.UsernameMaxLength]
            : trimmed;
    }

    private static bool IsAllowedChar(char c)
        => c is >= 'a' and <= 'z' or >= '0' and <= '9' or '_' or '-' or '.';

    [GeneratedRegex(@"[_\-\.]{2,}")]
    private static partial Regex UsernameRegex();
}
