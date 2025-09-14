using Hangfire;
using Moq;

namespace UserService.Tests.UnitTests.Configurations;

internal static class BackgroundJobClientConfiguration
{
    public static IBackgroundJobClient GetBackgroundJobClientConfiguration()
    {
        return new Mock<IBackgroundJobClient>().Object;
    }
}