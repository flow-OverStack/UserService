using Moq;
using UserService.Domain.Interfaces.Provider;

namespace UserService.Tests.UnitTests.Configurations;

internal static class IdentityCompensationQueueConfiguration
{
    public static Mock<IIdentityCompensationQueue> GetMockIdentityCompensationQueue()
    {
        return new Mock<IIdentityCompensationQueue>();
    }
}
