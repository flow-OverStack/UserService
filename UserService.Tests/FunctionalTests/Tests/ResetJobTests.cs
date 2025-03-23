using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserService.BackgroundJobs.Jobs;
using UserService.Domain.Entity;
using UserService.Domain.Events;
using UserService.Domain.Interfaces.Repositories;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

[Collection("ReputationResetSequentialTests")]
public class ResetJobTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task RunReputationResetJob_ShouldBe_Success()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var reputationResetService = scope.ServiceProvider.GetRequiredService<ReputationResetJob>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();

        //Act
        await reputationResetService.Run();

        //Assert
        var reputations = await userRepository.GetAll().Select(x => x.ReputationEarnedToday).ToListAsync();

        Assert.True(reputations.All(x => x == 0));
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RunProcessedEvent_ShouldBe_Success()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var processedEventsResetJob = scope.ServiceProvider.GetRequiredService<ProcessedEventsResetJob>();
        var eventRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ProcessedEvent>>();

        //Act
        await processedEventsResetJob.Run();

        //Assert
        var processedEvents = await eventRepository.GetAll().ToListAsync();

        Assert.True(processedEvents.All(x => x.ProcessedAt.AddDays(7) >= DateTime.UtcNow));
    }
}