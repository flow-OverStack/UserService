using Hangfire;
using MassTransit;
using MassTransit.Transports;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UserService.Domain.Enums;
using UserService.Messaging.Events;
using UserService.Messaging.Filters;
using UserService.Tests.Configurations;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class ResilientConsumeFilterTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Probe_ShouldBe_Ok()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var backgroundJob = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();
        var filter = new ResilientConsumeFilter<BaseEvent>(backgroundJob);
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
        var backgroundJob = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();
        var filter = new ResilientConsumeFilter<BaseEvent>(backgroundJob);

        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);
        contextMock.Setup(x => x.Headers)
            .Returns(new JsonTransportHeaders(new DictionaryHeaderProvider(new Dictionary<string, object>())));

        var pipeMock = new Mock<IPipe<ConsumeContext<BaseEvent>>>();

        //Act
        await filter.Send(contextMock.Object, pipeMock.Object);

        //Assert
        Assert.True(true);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Send_ShouldBe_Exception()
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
        var backgroundJob = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();
        var filter = new ResilientConsumeFilter<BaseEvent>(backgroundJob);

        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);
        contextMock.Setup(x => x.Headers)
            .Returns(new JsonTransportHeaders(new DictionaryHeaderProvider(new Dictionary<string, object>())));

        var pipeMock = new Mock<IPipe<ConsumeContext<BaseEvent>>>();
        pipeMock.Setup(x => x.Send(It.IsAny<ConsumeContext<BaseEvent>>())).ThrowsAsync(new TestException());

        //Act
        var action = async () => await filter.Send(contextMock.Object, pipeMock.Object);

        //Assert
        await Assert.ThrowsAsync<TestException>(action);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Send_ShouldBe_Exception_With_SuccessfulRetry()
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
        var backgroundJob = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();
        var filter = new ResilientConsumeFilter<BaseEvent>(backgroundJob);

        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);
        contextMock.Setup(x => x.Headers)
            .Returns(new JsonTransportHeaders(new DictionaryHeaderProvider(new Dictionary<string, object>())));

        var isThrown = false;
        var pipeMock = new Mock<IPipe<ConsumeContext<BaseEvent>>>();
        pipeMock.Setup(x => x.Send(It.IsAny<ConsumeContext<BaseEvent>>()))
            .Returns((ConsumeContext<BaseEvent> _) =>
            {
                if (isThrown)
                    return Task.CompletedTask;

                isThrown = true;
                throw new TestException();
            });

        //Act
        await filter.Send(contextMock.Object, pipeMock.Object);

        //Assert
        Assert.True(true);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Send_ShouldBe_MovedToDLQ()
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
        var backgroundJob = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();
        var filter = new ResilientConsumeFilter<BaseEvent>(backgroundJob);

        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);
        contextMock.Setup(x => x.Headers)
            .Returns(new JsonTransportHeaders(new DictionaryHeaderProvider(new Dictionary<string, object>
                { { "RedeliveryCount", "6" } })));
        contextMock.Setup(x => x.DestinationAddress).Returns(new Uri("queue:test-queue"));
        var pipeMock = new Mock<IPipe<ConsumeContext<BaseEvent>>>();
        pipeMock.Setup(x => x.Send(It.IsAny<ConsumeContext<BaseEvent>>())).ThrowsAsync(new TestException());

        //Act
        var action = async () => await filter.Send(contextMock.Object, pipeMock.Object);

        //Assert
        await Assert.ThrowsAsync<TestException>(action);
    }
}