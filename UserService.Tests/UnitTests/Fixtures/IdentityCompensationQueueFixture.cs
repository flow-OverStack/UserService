using Moq;
using UserService.Domain.Interfaces.Provider;

namespace UserService.Tests.UnitTests.Fixtures;

internal static class IdentityCompensationQueueFixture
{
    public static Mock<IIdentityCompensationQueue> GetMockIdentityCompensationQueue()
    {
        return new Mock<IIdentityCompensationQueue>();
    }
}
