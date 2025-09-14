using AutoMapper;
using Hangfire;
using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.Configurations;
using MapperConfiguration = UserService.Tests.UnitTests.Configurations.MapperConfiguration;

namespace UserService.Tests.UnitTests.Factories;

internal class AuthServiceFactory
{
    private readonly IAuthService _authService;

    public readonly IBackgroundJobClient BackgroundJob =
        BackgroundJobClientConfiguration.GetBackgroundJobClientConfiguration();

    public readonly IIdentityServer IdentityServer = IdentityServerConfiguration.GetIdentityServerConfiguration();
    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();
    public readonly IUnitOfWork UnitOfWork;


    public AuthServiceFactory(IBaseRepository<User>? userRepository = null,
        IBaseRepository<Role>? roleRepository = null)
    {
        UnitOfWork = MockRepositoriesGetters.GetMockUnitOfWork(userRepository, roleRepository).Object;

        _authService = new AuthService(Mapper, IdentityServer, UnitOfWork, BackgroundJob);
    }

    public IAuthService GetService()
    {
        return _authService;
    }
}