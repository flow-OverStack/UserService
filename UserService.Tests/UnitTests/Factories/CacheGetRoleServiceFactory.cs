using Microsoft.Extensions.Options;
using UserService.Application.Services;
using UserService.Application.Services.Cache;
using UserService.Cache.Providers;
using UserService.Cache.Repositories;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Settings;
using UserService.Tests.UnitTests.Configurations;

namespace UserService.Tests.UnitTests.Factories;

internal class CacheGetRoleServiceFactory
{
    private readonly IGetRoleService _cacheGetRoleService;

    public readonly GetRoleService InnerGetRoleService =
        (GetRoleService)new GetRoleServiceFactory().GetService();

    public readonly RedisSettings RedisSettings = RedisSettingsConfiguration.GetRedisSettingsConfiguration();

    public readonly IBaseCacheRepository<Role, long> RoleCacheRepository =
        new RoleCacheRepository(new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()));

    public CacheGetRoleServiceFactory()
    {
        _cacheGetRoleService = new CacheGetRoleService(RoleCacheRepository, InnerGetRoleService,
            new OptionsWrapper<RedisSettings>(RedisSettings));
    }

    public IGetRoleService GetService()
    {
        return _cacheGetRoleService;
    }
}