using Microsoft.Extensions.Options;
using UserService.Application.Services.Cache;
using UserService.Cache.Providers;
using UserService.Cache.Repositories;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.UnitTests.Configurations;

namespace UserService.Tests.UnitTests.Factories;

internal class CacheGetRoleServiceFactory
{
    private readonly IGetRoleService _cacheGetRoleService;

    public readonly IGetRoleService InnerGetRoleService = new GetRoleServiceFactory().GetService();

    public readonly IRoleCacheRepository RoleCacheRepository =
        new RoleCacheRepository(
            new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsConfiguration.GetRedisSettingsConfiguration()));

    public CacheGetRoleServiceFactory()
    {
        _cacheGetRoleService = new CacheGetRoleService(RoleCacheRepository, InnerGetRoleService);
    }

    public IGetRoleService GetService()
    {
        return _cacheGetRoleService;
    }
}