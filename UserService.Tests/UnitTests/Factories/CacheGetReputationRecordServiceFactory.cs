using Microsoft.Extensions.Options;
using UserService.Application.Services.Cache;
using UserService.Cache.Providers;
using UserService.Cache.Repositories;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.UnitTests.Configurations;

namespace UserService.Tests.UnitTests.Factories;

public class CacheGetReputationRecordServiceFactory
{
    private readonly IGetReputationRecordService _cacheGetReputationRecordService;

    public readonly IGetReputationRecordService InnerGetReputationRecordService =
        new GetReputationRecordServiceFactory().GetService();

    public readonly IReputationRecordCacheRepository ReputationRecordCacheRepository =
        new ReputationRecordCacheRepository(
            new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsConfiguration.GetRedisSettingsConfiguration()));

    public CacheGetReputationRecordServiceFactory()
    {
        _cacheGetReputationRecordService =
            new CacheGetReputationRecordService(ReputationRecordCacheRepository, InnerGetReputationRecordService);
    }

    public IGetReputationRecordService GetService()
    {
        return _cacheGetReputationRecordService;
    }
}