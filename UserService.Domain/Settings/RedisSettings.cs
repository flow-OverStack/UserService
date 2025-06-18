namespace UserService.Domain.Settings;

public class RedisSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Password { get; set; }
    public int TimeToLiveInSeconds { get; set; }
}