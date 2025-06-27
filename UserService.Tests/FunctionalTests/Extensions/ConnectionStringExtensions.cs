namespace UserService.Tests.FunctionalTests.Extensions;

internal static class ConnectionStringExtensions
{
    /// <summary>
    ///     Parses connection string into host and port
    /// </summary>
    /// <param name="s"></param>
    /// <param name="host"></param>
    /// <param name="port"></param>
    public static void ParseConnectionString(this string s, out string host, out int port)
    {
        ArgumentException.ThrowIfNullOrEmpty(s);

        var parts = s.Split(':');
        if (parts.Length != 2)
            throw new FormatException("Invalid format of connection string. Valid: host:port");

        host = parts[0];
        if (string.IsNullOrWhiteSpace(host))
            throw new FormatException("Host cannot be empty.");

        if (!int.TryParse(parts[1], out port))
            throw new FormatException("Port must be a valid integer.");
    }
}