using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using UserService.Application.Services.Cache;
using UserService.Cache.Providers;
using UserService.Cache.Repositories;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.UnitTests.Configurations;

namespace UserService.Tests.UnitTests.Factories;

public class CacheGetReputationRuleServiceFactory
{
    private readonly IGetReputationRuleService _cacheGetReputationRuleService;

    public readonly IGetReputationRuleService InnerGetReputationRuleService =
        new GetReputationRuleServiceFactory().GetService();

    public readonly IReputationRuleCacheRepository ReputationRuleCacheRepository =
        new ReputationRuleCacheRepository(
            new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsConfiguration.GetRedisSettingsConfiguration()),
            new Mock<ILogger>().Object);

    public CacheGetReputationRuleServiceFactory()
    {
        _cacheGetReputationRuleService =
            new CacheGetReputationRuleService(ReputationRuleCacheRepository, InnerGetReputationRuleService);
    }

    public IGetReputationRuleService GetService()
    {
        return _cacheGetReputationRuleService;
    }
}