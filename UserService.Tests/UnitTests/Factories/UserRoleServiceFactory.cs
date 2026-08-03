using Moq;
using UserService.Application.Services;
using UserService.Application.Services.Identity;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Configurations;

namespace UserService.Tests.UnitTests.Factories;

internal class UserRoleServiceFactory
{
    private readonly IUserRoleService _userRoleService;

    public readonly Mock<IIdentityRoleSynchronizer> Synchronizer = new();

    public readonly IUnitOfWork UnitOfWork = MockRepositoriesGetters.GetMockUnitOfWork().Object;

    public UserRoleServiceFactory()
    {
        _userRoleService = new UserRoleService(UnitOfWork, Synchronizer.Object);
    }

    public IUserRoleService GetService()
    {
        return _userRoleService;
    }
}
