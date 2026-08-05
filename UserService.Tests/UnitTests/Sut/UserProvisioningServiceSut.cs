using AutoMapper;
using UserService.Application.Services.Provisioning;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Repository;
using UserService.Tests.Mocks;
using UserService.Tests.UnitTests.Fixtures;

namespace UserService.Tests.UnitTests.Sut;

internal class UserProvisioningServiceSut
{
    private readonly IUserProvisioningService _provisioningService;

    public readonly IIdentityServer IdentityServer = IdentityServerFixture.GetIdentityServerConfiguration();

    public readonly IMapper Mapper = MapperFixture.GetMapperConfiguration();

    public readonly IBaseRepository<User> UserRepository = RepositoryMocks.GetMockUserRepository().Object;

    public readonly IUnitOfWork UnitOfWork;

    public readonly IUsernameGenerator UsernameGenerator;

    public UserProvisioningServiceSut(IBaseRepository<User>? userRepository = null,
        IBaseRepository<Role>? roleRepository = null)
    {
        UnitOfWork = RepositoryMocks.GetMockUnitOfWork(userRepository, roleRepository).Object;
        UsernameGenerator = new UsernameGenerator(userRepository ?? UserRepository);

        _provisioningService = new UserProvisioningService(Mapper, IdentityServer, UnitOfWork, UsernameGenerator);
    }

    public IUserProvisioningService GetService()
    {
        return _provisioningService;
    }
}
