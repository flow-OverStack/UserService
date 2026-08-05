using AutoMapper;
using Moq;
using UserService.Application.Services;
using UserService.Application.Services.Identity;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Mocks;
using UserService.Tests.UnitTests.Fixtures;

namespace UserService.Tests.UnitTests.Sut;

internal class RoleServiceSut
{
    private readonly IRoleService _roleService;

    public readonly IMapper Mapper = MapperFixture.GetMapperConfiguration();

    public readonly IIdentityRoleSynchronizer Synchronizer = new Mock<IIdentityRoleSynchronizer>().Object;

    public readonly IUnitOfWork UnitOfWork = RepositoryMocks.GetMockUnitOfWork().Object;

    public RoleServiceSut()
    {
        _roleService = new RoleService(Mapper, UnitOfWork, Synchronizer);
    }

    public IRoleService GetService()
    {
        return _roleService;
    }
}
