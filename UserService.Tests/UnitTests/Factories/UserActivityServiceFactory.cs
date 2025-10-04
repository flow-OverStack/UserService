using AutoMapper;
using StackExchange.Redis;
using UserService.Application.Services;
using UserService.Cache.Providers;
using UserService.Cache.Repositories;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.Configurations;
using MapperConfiguration = UserService.Tests.UnitTests.Configurations.MapperConfiguration;

namespace UserService.Tests.UnitTests.Factories;

internal class UserActivityServiceFactory
{
    private readonly IUserActivityDatabaseService _userActivityDatabaseService;
    private readonly IUserActivityService _userActivityService;

    public readonly IUserActivityCacheRepository CacheRepository =
        new UserActivityCacheRepository(
            new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()));

    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();

    public readonly IBaseRepository<User> UserRepository = MockRepositoriesGetters.GetMockUserRepository().Object;

    public UserActivityServiceFactory(IDatabase? redisDatabase = null)
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