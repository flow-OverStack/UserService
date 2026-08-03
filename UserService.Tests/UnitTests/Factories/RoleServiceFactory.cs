using AutoMapper;
using Moq;
using UserService.Application.Services;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.Configurations;
using MapperConfiguration = UserService.Tests.UnitTests.Configurations.MapperConfiguration;

namespace UserService.Tests.UnitTests.Factories;

internal class RoleServiceFactory
{
    private readonly IRoleService _roleService;

    public readonly Mock<IIdentityCompensationQueue> CompensationQueue =
        IdentityCompensationQueueConfiguration.GetMockIdentityCompensationQueue();

    public readonly IIdentityServer IdentityServer = IdentityServerConfiguration.GetIdentityServerConfiguration();
    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();

    public readonly IUnitOfWork UnitOfWork = MockRepositoriesGetters.GetMockUnitOfWork().Object;

    public RoleServiceFactory()
    {
        _roleService = new RoleService(Mapper, UnitOfWork, IdentityServer, CompensationQueue.Object);
    }

    public IRoleService GetService()
    {
        return _roleService;
    }
}
