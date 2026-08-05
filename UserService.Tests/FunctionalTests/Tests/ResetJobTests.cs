using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserService.BackgroundJobs.Jobs;
using UserService.Domain.Interfaces.Repository;
using UserService.Messaging.Events;
using UserService.Tests.FunctionalTests.Base;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.FunctionalTests.Tests;

[Collection(nameof(ResetJobTests))]
[FunctionalTest]
public class ResetJobTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task RunAsync_ExistingProcessedEvents_ReturnsOk()
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