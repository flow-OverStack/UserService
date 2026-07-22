using AutoMapper;
using Hangfire;
using UserService.Application.Services;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Mocks;
using UserService.Tests.UnitTests.Fixtures;

namespace UserService.Tests.UnitTests.Sut;

internal class RoleServiceSut
{
    private readonly IRoleService _roleService;

    public readonly IBackgroundJobClient BackgroundJob =
        BackgroundJobClientFixture.GetBackgroundJobClientConfiguration();

    public readonly IIdentityServer IdentityServer = IdentityServerFixture.GetIdentityServerConfiguration();
    public readonly IMapper Mapper = MapperFixture.GetMapperConfiguration();

    public readonly IUnitOfWork UnitOfWork = RepositoryMocks.GetMockUnitOfWork().Object;

    public RoleServiceSut()
    {
        _roleService = new RoleService(Mapper, UnitOfWork, IdentityServer, BackgroundJob);
    }

    public IRoleService GetService()
    {
        return _roleService;
    }
}
