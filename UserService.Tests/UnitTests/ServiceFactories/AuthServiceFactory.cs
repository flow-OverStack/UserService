using AutoMapper;
using UserService.Application.Services;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Tests.UnitTests.Configurations;
using MapperConfiguration = UserService.Tests.UnitTests.Configurations.MapperConfiguration;

namespace UserService.Tests.UnitTests.ServiceFactories;

internal class AuthServiceFactory
{
    private readonly IAuthService _authService;
    public readonly IIdentityServer IdentityServer = IdentityServerConfiguration.GetIdentityServerConfiguration();
    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();
    public readonly IBaseRepository<Role> RoleRepository = MockRepositoriesGetters.GetMockRoleRepository().Object;
    public readonly IUnitOfWork UnitOfWork = MockRepositoriesGetters.GetMockUnitOfWork().Object;

    public readonly IBaseRepository<User> UserRepository = MockRepositoriesGetters.GetMockUserRepository().Object;

    public readonly IBaseRepository<UserToken> UserTokenRepository =
        MockRepositoriesGetters.GetMockUserTokenRepository().Object;


    public AuthServiceFactory()
    {
        _authService = new AuthService(UserRepository, Mapper, IdentityServer, RoleRepository, UnitOfWork,
            UserTokenRepository);
    }

    public IAuthService GetService()
    {
        return _authService;
    }
}