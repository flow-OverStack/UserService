using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserService.BackgroundJobs.Jobs;
using UserService.DAL;
using UserService.Domain.Entity;
using UserService.Domain.Entity.Events;
using UserService.Tests.FunctionalTests.Base.Exception.ResetJob;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class ExceptionResetJobTests(ResetJobExceptionFunctionalTestWebAppFactory factory)
    : ResetJobExceptionBaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task RunReputationResetJob_ShouldBe_NoException()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var reputationResetJob = scope.ServiceProvider.GetRequiredService<ReputationResetJob>();
        //ApplicationDbContext is instead of IBaseRepository<User> because IBaseRepository<User> is mocked 
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        //Act
        await reputationResetJob.Run();

        //Assert
        var reputations = await dbContext.Set<User>().Select(x => x.ReputationEarnedToday).ToListAsync();

        Assert.True(reputations.Any(x => x != 0));
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RunProcessedEventResetJob_ShouldBe_NoException()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var processedEventsResetJob = scope.ServiceProvider.GetRequiredService<ProcessedEventsResetJob>();
        //ApplicationDbContext is instead of IBaseRepository<User> because IBaseRepository<User> is mocked 
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        //Act
        await processedEventsResetJob.Run();

        //Assert
        var processedEvents = await dbContext.Set<ProcessedEvent>().ToListAsync();

        Assert.True(processedEvents.Any(x => x.ProcessedAt.AddDays(7) < DateTime.UtcNow));
    }
}