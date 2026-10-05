using System.Runtime.CompilerServices;
using Serilog;
using StackExchange.Redis;

namespace UserService.Cache.Extensions;

public static class RedisLoggerExtensions
{
    private const string FailureMessageTemplate = "Redis operation {Operation} failed for keys {Keys}";

    public static bool IsRedisFailure(this Exception exception)
    {
        return exception is RedisException or RedisTimeoutException;
    }

    public static void LogRedisFailure(this ILogger logger, Exception exception, IEnumerable<string> keys,
        [CallerMemberName] string operation = "")
    {
        if (exception is RedisConnectionException)
            logger.Debug(FailureMessageTemplate + ": {Reason}", operation, keys, exception.Message);
        else
            logger.Warning(exception, FailureMessageTemplate, operation, keys);
    }
}