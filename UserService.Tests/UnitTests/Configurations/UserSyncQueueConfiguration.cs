using Moq;
using UserService.Domain.Interfaces.Provider;

namespace UserService.Tests.UnitTests.Configurations;

internal static class UserSyncQueueConfiguration
{
    public static Mock<IUserSyncQueue> GetMockUserSyncQueue()
    {
        return new Mock<IUserSyncQueue>();
    }
}
