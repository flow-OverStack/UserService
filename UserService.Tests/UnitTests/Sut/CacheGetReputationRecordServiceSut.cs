using Microsoft.Extensions.Options;
using UserService.Application.Services.Cache;
using UserService.Cache.Providers;
using UserService.Cache.Repositories;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.UnitTests.Fixtures;

namespace UserService.Tests.UnitTests.Sut;

public class CacheGetReputationRecordServiceSut
{
    private readonly IGetReputationRecordService _cacheGetReputationRecordService;

    public readonly IGetReputationRecordService InnerGetReputationRecordService =
        new GetReputationRecordServiceSut().GetService();

    public readonly IReputationRecordCacheRepository ReputationRecordCacheRepository =
        new ReputationRecordCacheRepository(
            new RedisCacheProvider(RedisDatabaseFixture.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsFixture.GetRedisSettingsConfiguration()));

    public CacheGetReputationRecordServiceSut()
    {
        _cacheGetReputationRecordService =
            new CacheGetReputationRecordService(ReputationRecordCacheRepository, InnerGetReputationRecordService);
    }

    public IGetReputationRecordService GetService()
    {
        return _cacheGetReputationRecordService;
    }
}
