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

internal class CacheGetUserServiceFactory
{
    private readonly IGetUserService _cacheGetUserService;

    public readonly GetRoleService InnerGetRoleService =
        (GetRoleService)new GetRoleServiceFactory().GetService();

    public readonly GetUserService InnerGetUserService =
        (GetUserService)new GetUserServiceFactory().GetService();

    public readonly RedisSettings RedisSettings = RedisSettingsConfiguration.GetRedisSettingsConfiguration();

    public readonly IBaseCacheRepository<Role, long> RoleCacheRepository =
        new RoleCacheRepository(new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()));

    public readonly IBaseCacheRepository<User, long> UserCacheRepository =
        new UserCacheRepository(new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()));

    public CacheGetUserServiceFactory()
    {
        _cacheGetUserService = new CacheGetUserService(UserCacheRepository, InnerGetUserService, RoleCacheRepository,
            InnerGetRoleService, new OptionsWrapper<RedisSettings>(RedisSettings));
    }

    public IGetUserService GetService()
    {
        return _cacheGetUserService;
    }
}