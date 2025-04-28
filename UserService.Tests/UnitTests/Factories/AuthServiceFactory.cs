using AutoMapper;
using UserService.Application.Services;
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
    public readonly IUnitOfWork UnitOfWork = MockRepositoriesGetters.GetMockUnitOfWork().Object;


    public AuthServiceFactory(IUnitOfWork? unitOfWork = null)
    {
        if (unitOfWork != null)
            UnitOfWork = unitOfWork;

        _authService = new AuthService(Mapper, IdentityServer, UnitOfWork);
    }

    public IAuthService GetService()
    {
        return _authService;
    }
}