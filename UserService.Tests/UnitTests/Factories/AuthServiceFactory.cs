using AutoMapper;
using UserService.Application.Services;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.Configurations;
using MapperConfiguration = UserService.Tests.UnitTests.Configurations.MapperConfiguration;

namespace UserService.Tests.UnitTests.Factories;

internal class AuthServiceFactory
{
    private readonly IAuthService _authService;
    public readonly IIdentityServer IdentityServer = IdentityServerConfiguration.GetIdentityServerConfiguration();
    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();
    public readonly IBaseRepository<Role> RoleRepository = MockRepositoriesGetters.GetMockRoleRepository().Object;
    public readonly IUnitOfWork UnitOfWork = MockRepositoriesGetters.GetMockUnitOfWork().Object;

    public readonly IBaseRepository<User> UserRepository = MockRepositoriesGetters.GetMockUserRepository().Object;


    public AuthServiceFactory(IUnitOfWork? unitOfWork = null, IBaseRepository<Role>? roleRepository = null)
    {
        if (unitOfWork != null)
            UnitOfWork = unitOfWork;

        if (roleRepository != null)
            RoleRepository = roleRepository;

        _authService = new AuthService(UserRepository, Mapper, IdentityServer, RoleRepository, UnitOfWork);
    }

    public IAuthService GetService()
    {
        return _authService;
    }
}