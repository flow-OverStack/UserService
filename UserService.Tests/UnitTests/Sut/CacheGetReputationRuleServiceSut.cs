using Microsoft.Extensions.Options;
using UserService.Application.Services.Cache;
using UserService.Cache.Providers;
using UserService.Cache.Repositories;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.UnitTests.Fixtures;

namespace UserService.Tests.UnitTests.Sut;

public class CacheGetReputationRuleServiceSut
{
    private readonly IGetReputationRuleService _cacheGetReputationRuleService;

    public readonly IGetReputationRuleService InnerGetReputationRuleService =
        new GetReputationRuleServiceSut().GetService();

    public readonly IReputationRuleCacheRepository ReputationRuleCacheRepository =
        new ReputationRuleCacheRepository(
            new RedisCacheProvider(RedisDatabaseFixture.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsFixture.GetRedisSettingsConfiguration()));

    public CacheGetReputationRuleServiceSut()
    {
        _cacheGetReputationRuleService =
            new CacheGetReputationRuleService(ReputationRuleCacheRepository, InnerGetReputationRuleService);
    }

    public IGetReputationRuleService GetService()
    {
        return _cacheGetReputationRuleService;
    }
}
