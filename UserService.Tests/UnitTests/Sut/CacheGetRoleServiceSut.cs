using Microsoft.Extensions.Options;
using UserService.Application.Services.Cache;
using UserService.Cache.Providers;
using UserService.Cache.Repositories;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.UnitTests.Fixtures;

namespace UserService.Tests.UnitTests.Sut;

internal class CacheGetRoleServiceSut
{
    private readonly IGetRoleService _cacheGetRoleService;

    public readonly IGetRoleService InnerGetRoleService = new GetRoleServiceSut().GetService();

    public readonly IRoleCacheRepository RoleCacheRepository =
        new RoleCacheRepository(
            new RedisCacheProvider(RedisDatabaseFixture.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsFixture.GetRedisSettingsConfiguration()));

    public CacheGetRoleServiceSut()
    {
        _cacheGetRoleService = new CacheGetRoleService(RoleCacheRepository, InnerGetRoleService);
    }

    public IGetRoleService GetService()
    {
        return _cacheGetRoleService;
    }
}
