using Microsoft.Extensions.Options;
using UserService.Application.Services.Cache;
using UserService.Cache.Providers;
using UserService.Cache.Repositories;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.UnitTests.Fixtures;

namespace UserService.Tests.UnitTests.Sut;

internal class CacheGetUserServiceSut
{
    private readonly IGetUserService _cacheGetUserService;

    public readonly IGetUserService InnerGetUserService = new GetUserServiceSut().GetService();

    public readonly IUserCacheRepository UserCacheRepository =
        new UserCacheRepository(
            new RedisCacheProvider(RedisDatabaseFixture.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsFixture.GetRedisSettingsConfiguration()));

    public CacheGetUserServiceSut()
    {
        _cacheGetUserService = new CacheGetUserService(UserCacheRepository, InnerGetUserService);
    }

    public IGetUserService GetService()
    {
        return _cacheGetUserService;
    }
}
