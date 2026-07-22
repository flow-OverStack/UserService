using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserService.BackgroundJobs.Jobs;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.FunctionalTests.Tests;

[FunctionalTest]
public class SyncUserActivitiesJobTests(FunctionalTestWebAppFactory factory) : SequentialFunctionalTest(factory)
{
    [Fact]
    public async Task RunAsync_ValidCachedActivities_ReturnsOk()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var cache = scope.ServiceProvider.GetRequiredService<ICacheProvider>();
        var job = ActivatorUtilities.CreateInstance<SyncUserActivitiesJob>(scope.ServiceProvider);
        var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();
        var initialLastLoginAt = await userRepository.GetAll().Select(x => x.LastLoginAt).ToArrayAsync();

        await cache.InsertActivities();

        //Act
        await job.RunAsync();

        //Assert
        var finalLastLoginAt = await userRepository.GetAll().Select(x => x.LastLoginAt).ToArrayAsync();
        Assert.Equal(2, finalLastLoginAt.Except(initialLastLoginAt).Count()); // There are 2 new heartbeats in total
    }

    [Fact]
    public async Task RunAsync_InvalidCachedActivities_ReturnsNoSyncedActivities()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var cache = scope.ServiceProvider.GetRequiredService<ICacheProvider>();
        var job = ActivatorUtilities.CreateInstance<SyncUserActivitiesJob>(scope.ServiceProvider);
        var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();
        var initialLastLoginAt = await userRepository.GetAll().Select(x => x.LastLoginAt).ToArrayAsync();

        await cache.InsertInvalidActivities();

        //Act
        await job.RunAsync();

        //Assert
        var finalLastLoginAt = await userRepository.GetAll().Select(x => x.LastLoginAt).ToArrayAsync();
        Assert.Empty(finalLastLoginAt.Except(initialLastLoginAt)); // There are no new heartbeats in total
    }
}