using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserService.BackgroundJobs.Jobs;
using UserService.Domain.Interfaces.Repository;
using UserService.Messaging.Events;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

[Collection("ReputationResetSequentialTests")]
public class ResetJobTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task RunProcessedEvent_ShouldBe_Ok()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var processedEventsResetJob = ActivatorUtilities.CreateInstance<ProcessedEventsResetJob>(scope.ServiceProvider);
        var eventRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ProcessedEvent>>();

        //Act
        await processedEventsResetJob.RunAsync();

        //Assert
        var processedEvents = await eventRepository.GetAll().ToListAsync();

        Assert.True(processedEvents.All(x => x.ProcessedAt.AddDays(7) >= DateTime.UtcNow));
    }
}