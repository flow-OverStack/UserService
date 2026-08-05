using AutoMapper;
using UserService.Application.Services;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.UnitTests.Fixtures;

namespace UserService.Tests.UnitTests.Sut;

internal class TokenServiceSut
{
    private readonly ITokenService _tokenService;
    public readonly IIdentityServer IdentityServer = IdentityServerFixture.GetIdentityServerConfiguration();
    public readonly IMapper Mapper = MapperFixture.GetMapperConfiguration();

    public TokenServiceSut()
    {
        _tokenService = new TokenService(IdentityServer, Mapper);
    }

    public ITokenService GetService()
    {
        return _tokenService;
    }
}
