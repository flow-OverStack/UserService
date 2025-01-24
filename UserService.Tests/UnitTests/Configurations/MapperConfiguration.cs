using AutoMapper;
using UserService.Application.Mapping;

namespace UserService.Tests.UnitTests.Configurations;

internal static class MapperConfiguration
{
    public static IMapper GetMapperConfiguration()
    {
        var mockMapper = new AutoMapper.MapperConfiguration(cfg => cfg.AddMaps(typeof(UserMapping)));
        return mockMapper.CreateMapper();
    }
}