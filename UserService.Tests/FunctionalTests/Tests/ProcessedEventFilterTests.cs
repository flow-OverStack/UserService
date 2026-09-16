using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Repository;
using UserService.Messaging.Events;
using UserService.Messaging.Filters;
using UserService.Tests.FunctionalTests.Base;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.FunctionalTests.Tests;

[FunctionalTest]
public class ProcessedEventFilterTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task Probe_ValidProbeContext_ReturnsOk()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var filter = ActivatorUtilities.CreateInstance<ProcessedEventFilter<BaseEvent>>(scope.ServiceProvider);
        var probeContext = new Mock<ProbeContext>();
        probeContext.Setup(x => x.CreateScope(It.IsAny<string>())).Returns(new Mock<ProbeContext>().Object);

        //Act
        filter.Probe(probeContext.Object);

        //Assert
        Assert.True(true);
    }

    [Fact]
    public async Task Send_NewEvent_ReturnsOk()
    {
        //Arrange
        const long authorId = 1;
        const long entityId = 2;
        const long initiatorId = 1;

        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.EntityAccepted),
            EntityType = nameof(EntityType.Answer),
            AuthorId = authorId,
            EntityId = entityId,
            InitiatorId = initiatorId,
            EventId = Guid.NewGuid()
        };
        await using var scope = ServiceProvider.CreateAsyncScope();
        var filter = ActivatorUtilities.CreateInstance<ProcessedEventFilter<BaseEvent>>(scope.ServiceProvider);

        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        var pipeMock = new Mock<IPipe<ConsumeContext<BaseEvent>>>();

        //Act
        await filter.Send(contextMock.Object, pipeMock.Object);

        //Assert
        Assert.True(true);
    }

    [Fact]
    public async Task Send_DuplicateEvent_ReturnsSkippedDuplicate()
    {
        //Arrange
        const long authorId = 1;
        const long entityId = 2;
        const long initiatorId = 1;
        var id = Guid.NewGuid();

        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.EntityAccepted),
            EntityType = nameof(EntityType.Answer),
            AuthorId = authorId,
            InitiatorId = initiatorId,
            EntityId = entityId,
            EventId = id
        };
        await using var scope = ServiceProvider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ProcessedEvent>>();
        var filter = ActivatorUtilities.CreateInstance<ProcessedEventFilter<BaseEvent>>(scope.ServiceProvider);

        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        var pipeMock = new Mock<IPipe<ConsumeContext<BaseEvent>>>();

        await filter.Send(contextMock.Object, pipeMock.Object);

        var initialEventCount = await repository.GetAll().AsNoTracking().CountAsync();

        //Act
        await filter.Send(contextMock.Object, pipeMock.Object);

        //Assert
        var updatedEventCount = await repository.GetAll().AsNoTracking().CountAsync();
        Assert.Equal(initialEventCount, updatedEventCount);
    }
}