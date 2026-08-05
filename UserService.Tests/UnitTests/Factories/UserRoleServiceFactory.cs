using AutoMapper;
using Moq;
using UserService.Application.Services;
using UserService.Application.Services.Identity;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Configurations;
using MapperConfiguration = UserService.Tests.UnitTests.Configurations.MapperConfiguration;

namespace UserService.Tests.UnitTests.Factories;

internal class UserRoleServiceFactory
{
    private readonly IUserRoleService _userRoleService;

    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();

    public readonly Mock<IIdentityRoleSynchronizer> Synchronizer = new();

    public readonly IUnitOfWork UnitOfWork = MockRepositoriesGetters.GetMockUnitOfWork().Object;

    public UserRoleServiceFactory()
    {
        _userRoleService = new UserRoleService(Mapper, UnitOfWork, Synchronizer.Object);
    }

    public IUserRoleService GetService()
    {
        return _userRoleService;
    }
}
