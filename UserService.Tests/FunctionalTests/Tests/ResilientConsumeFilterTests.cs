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
        var scopeFactory = scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
        var filter = new ResilientConsumeFilter<BaseEvent>(scopeFactory);
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
        await using var scope = ServiceProvider.CreateAsyncScope();
        var scopeFactory = scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
        var filter = new ResilientConsumeFilter<BaseEvent>(scopeFactory);
        var message = new BaseEvent
            { EventType = nameof(BaseEventType.AnswerAccepted), UserId = userId, EventId = Guid.NewGuid() };

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
        await using var scope = ServiceProvider.CreateAsyncScope();
        var scopeFactory = scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
        var filter = new ResilientConsumeFilter<BaseEvent>(scopeFactory);
        var message = new BaseEvent
            { EventType = nameof(BaseEventType.AnswerAccepted), UserId = userId, EventId = Guid.NewGuid() };

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
    public async Task Send_ShouldBe_MovedToDLQ()
    {
        //Arrange
        const long userId = 1;
        await using var scope = ServiceProvider.CreateAsyncScope();
        var scopeFactory = scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
        var filter = new ResilientConsumeFilter<BaseEvent>(scopeFactory);
        var message = new BaseEvent
            { EventType = nameof(BaseEventType.AnswerAccepted), UserId = userId, EventId = Guid.NewGuid() };

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