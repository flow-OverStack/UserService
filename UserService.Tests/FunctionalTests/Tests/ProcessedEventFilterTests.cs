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

namespace UserService.Tests.FunctionalTests.Tests;

public class ProcessedEventFilterTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Probe_ShouldBe_Ok()
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

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Send_ShouldBe_Ok()
    {
        //Arrange
        const long userId = 1;
        const long entityId = 2;

        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.AnswerAccepted),
            EntityType = nameof(EntityType.Answer),
            UserId = userId,
            EntityId = entityId,
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

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Send_ShouldBe_EventAlreadyProcessed()
    {
        //Arrange
        const long userId = 1;
        const long entityId = 2;
        var id = Guid.NewGuid();

        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.AnswerAccepted),
            EntityType = nameof(EntityType.Answer),
            UserId = userId,
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