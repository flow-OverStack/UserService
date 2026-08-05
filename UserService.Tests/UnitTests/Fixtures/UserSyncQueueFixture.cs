using Moq;
using UserService.Domain.Interfaces.Provider;

namespace UserService.Tests.UnitTests.Fixtures;

internal static class UserSyncQueueFixture
{
    public static Mock<IUserSyncQueue> GetMockUserSyncQueue()
    {
        return new Mock<IUserSyncQueue>();
    }
}
