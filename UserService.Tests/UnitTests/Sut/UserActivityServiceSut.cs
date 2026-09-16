using AutoMapper;
using StackExchange.Redis;
using UserService.Application.Services;
using UserService.Cache.Providers;
using UserService.Cache.Repositories;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Mocks;
using UserService.Tests.UnitTests.Fixtures;

namespace UserService.Tests.UnitTests.Sut;

internal class UserActivityServiceSut
{
    private readonly IUserActivityDatabaseService _userActivityDatabaseService;
    private readonly IUserActivityService _userActivityService;

    public readonly IUserActivityCacheRepository CacheRepository =
        new UserActivityCacheRepository(
            new RedisCacheProvider(RedisDatabaseFixture.GetRedisDatabaseConfiguration()));

    public readonly IMapper Mapper = MapperFixture.GetMapperConfiguration();

    public readonly IBaseRepository<User> UserRepository = RepositoryMocks.GetMockUserRepository().Object;

    public UserActivityServiceSut(IDatabase? redisDatabase = null)
    {
        if (redisDatabase != null)
            CacheRepository = new UserActivityCacheRepository(new RedisCacheProvider(redisDatabase));

        var service = new UserActivityService(CacheRepository, UserRepository, Mapper);

        _userActivityService = service;
        _userActivityDatabaseService = service;
    }

    public IUserActivityService GetService()
    {
        return _userActivityService;
    }

    public IUserActivityDatabaseService GetDatabaseService()
    {
        return _userActivityDatabaseService;
    }
}
