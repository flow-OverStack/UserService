using Microsoft.Extensions.Options;
using UserService.Application.Services;
using UserService.Application.Services.Cache;
using UserService.Cache.Providers;
using UserService.Cache.Repositories;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.UnitTests.Configurations;

namespace UserService.Tests.UnitTests.Factories;

internal class CacheGetUserServiceFactory
{
    private readonly IGetUserService _cacheGetUserService;

    public readonly GetUserService InnerGetUserService =
        (GetUserService)new GetUserServiceFactory().GetService();

    public readonly IUserCacheRepository UserCacheRepository =
        new UserCacheRepository(
            new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsConfiguration.GetRedisSettingsConfiguration()),
            (GetUserService)new GetUserServiceFactory().GetService());

    public CacheGetUserServiceFactory()
    {
        _cacheGetUserService = new CacheGetUserService(UserCacheRepository, InnerGetUserService);
    }

    public IGetUserService GetService()
    {
        return _cacheGetUserService;
    }
}