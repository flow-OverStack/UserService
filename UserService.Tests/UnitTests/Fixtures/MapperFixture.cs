using AutoMapper;
using UserService.Application.Mappings;
using UserService.Keycloak.Mappings;

namespace UserService.Tests.UnitTests.Fixtures;

internal static class MapperFixture
{
    public static IMapper GetMapperConfiguration()
    {
        var mockMapper = new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(UserMapping));
            cfg.AddMaps(typeof(KeycloakUserMapping));
        });
        return mockMapper.CreateMapper();
    }
}