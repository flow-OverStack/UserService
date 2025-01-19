using AutoMapper;
using UserService.Application.Services;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Tests.UnitTests.Configurations;
using MapperConfiguration = UserService.Tests.UnitTests.Configurations.MapperConfiguration;

namespace UserService.Tests.UnitTests.ServiceFactories;

public class TokenServiceFactory
{
    private readonly ITokenService _tokenService;
    public readonly IIdentityServer IdentityServer = IdentityServerConfiguration.GetIdentityServerConfiguration();
    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();

    public readonly IBaseRepository<User> UserRepository = MockRepositoriesGetters.GetMockUserRepository().Object;

    public TokenServiceFactory()
    {
        _tokenService = new TokenService(UserRepository, IdentityServer, Mapper);
    }

    public ITokenService GetService()
    {
        return _tokenService;
    }
}