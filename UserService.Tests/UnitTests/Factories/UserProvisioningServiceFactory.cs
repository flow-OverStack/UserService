using AutoMapper;
using UserService.Application.Services.Provisioning;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Repository;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.Configurations;
using MapperConfiguration = UserService.Tests.UnitTests.Configurations.MapperConfiguration;

namespace UserService.Tests.UnitTests.Factories;

internal class UserProvisioningServiceFactory
{
    private readonly IUserProvisioningService _provisioningService;

    public readonly IIdentityServer IdentityServer = IdentityServerConfiguration.GetIdentityServerConfiguration();

    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();
    
    public readonly IBaseRepository<User> UserRepository = MockRepositoriesGetters.GetMockUserRepository().Object;

    public readonly IUnitOfWork UnitOfWork;

    public readonly IUsernameGenerator UsernameGenerator;

    public UserProvisioningServiceFactory(IBaseRepository<User>? userRepository = null,
        IBaseRepository<Role>? roleRepository = null)
    {
        UnitOfWork = MockRepositoriesGetters.GetMockUnitOfWork(userRepository, roleRepository).Object;
        UsernameGenerator = new UsernameGenerator(userRepository ?? UserRepository);

        _provisioningService = new UserProvisioningService(Mapper, IdentityServer, UnitOfWork, UsernameGenerator);
    }

    public IUserProvisioningService GetService()
    {
        return _provisioningService;
    }
}
