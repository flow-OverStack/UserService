using AutoMapper;
using Moq;
using UserService.Application.Services;
using UserService.Application.Services.Identity;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Mocks;
using UserService.Tests.UnitTests.Fixtures;

namespace UserService.Tests.UnitTests.Sut;

internal class UserRoleServiceSut
{
    private readonly IUserRoleService _userRoleService;

    public readonly IMapper Mapper = MapperFixture.GetMapperConfiguration();

    public readonly Mock<IIdentityRoleSynchronizer> Synchronizer = new();

    public readonly IUnitOfWork UnitOfWork = RepositoryMocks.GetMockUnitOfWork().Object;

    public UserRoleServiceSut()
    {
        _userRoleService = new UserRoleService(Mapper, UnitOfWork, Synchronizer.Object);
    }

    public IUserRoleService GetService()
    {
        return _userRoleService;
    }
}
