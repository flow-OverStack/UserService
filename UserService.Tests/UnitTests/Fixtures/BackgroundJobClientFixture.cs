using Hangfire;
using Moq;

namespace UserService.Tests.UnitTests.Fixtures;

internal static class BackgroundJobClientFixture
{
    public static IBackgroundJobClient GetBackgroundJobClientConfiguration()
    {
        return new Mock<IBackgroundJobClient>().Object;
    }
}