using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Events;
using UserService.Domain.Interfaces.Repository;
using UserService.Tests.Configurations;
using UserService.Tests.FunctionalTests.Base.Kafka;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class ReputationConsumerTests(ReputationConsumerFunctionalTestWebAppFactory factory)
    : ReputationConsumerBaseFunctionalTest(factory)
{
    public static TheoryData<string> GetPositiveEvents()
    {
        return
        [
            BaseEventType.AnswerAccepted.ToString(), BaseEventType.AnswerUpvote.ToString(),
            BaseEventType.QuestionUpvote.ToString(), BaseEventType.UserAcceptedAnswer.ToString()
        ];
    }

    public static TheoryData<string> GetNegativeEvents()
    {
        return
        [
            BaseEventType.AnswerDownvote.ToString(), BaseEventType.DownvoteGivenForAnswer.ToString(),
            BaseEventType.QuestionDownvote.ToString()
        ];
    }

    [Trait("Category", "Functional")]
    [Theory]
    [MemberData(nameof(GetPositiveEvents))]
    public async Task ConsumePositiveEvent_ShouldBe_Success(string eventName)
    {
        //Arrange
        const long userId = 1;
        var message = new BaseEvent { EventType = eventName, UserId = userId, EventId = Guid.NewGuid() };
        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        using var scope = ServiceProvider.CreateScope();
        var reputationEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();

        var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == userId);
        var initialReputation = user!.Reputation;
        var initialReputationEarnedToday = user.ReputationEarnedToday;

        //Act
        await reputationEventConsumer.Consume(contextMock.Object);

        //Assert
        Assert.NotNull(user);
        Assert.True(user.Reputation > initialReputation);
        Assert.True(user.ReputationEarnedToday > initialReputationEarnedToday);
    }

    [Trait("Category", "Functional")]
    [Theory]
    [MemberData(nameof(GetNegativeEvents))]
    public async Task ConsumeNegativeEvent_ShouldBe_Success(string eventName)
    {
        //Arrange
        const long userId = 3;
        var message = new BaseEvent { EventType = eventName, UserId = userId, EventId = Guid.NewGuid() };
        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        using var scope = ServiceProvider.CreateScope();
        var reputationEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();

        var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == userId);
        var initialReputation = user!.Reputation;
        var initialReputationEarnedToday = user.ReputationEarnedToday;

        //Act
        await reputationEventConsumer.Consume(contextMock.Object);

        //Assert
        Assert.NotNull(user);
        Assert.True(user.Reputation < initialReputation);
        Assert.Equal(initialReputationEarnedToday, user.ReputationEarnedToday);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeEvent_ShouldBe_EventAlreadyProcessed()
    {
        //Arrange
        const long userId = 1;
        var message = new BaseEvent { EventType = "AnswerAccepted", UserId = userId, EventId = Guid.NewGuid() };
        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        using var scope = ServiceProvider.CreateScope();
        var reputationEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();

        await reputationEventConsumer.Consume(contextMock.Object); // Consuming 1st message

        var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == userId);
        var initialReputation = user!.Reputation;

        //Act

        await reputationEventConsumer.Consume(contextMock.Object); // Consuming duplicate

        //Assert
        Assert.NotNull(user);
        Assert.Equal(initialReputation, user.Reputation);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumePositiveEvent_ShouldBe_DailyReputationLimitExceeded()
    {
        //Arrange
        const long userId = 3;
        var message = new BaseEvent { EventType = "AnswerAccepted", UserId = userId, EventId = Guid.NewGuid() };
        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        using var scope = ServiceProvider.CreateScope();
        var reputationEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();

        var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == userId);
        var initialReputation = user!.Reputation;

        //Act
        await reputationEventConsumer.Consume(contextMock.Object);

        //Assert
        Assert.NotNull(user);
        Assert.Equal(MockRepositoriesGetters.MaxDailyReputation, user.ReputationEarnedToday);
        Assert.Equal(initialReputation, user.Reputation);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeNegativeEvent_ShouldBe_ReputationMinimumReached()
    {
        //Arrange
        const long userId = 1;
        var message = new BaseEvent { EventType = "AnswerDownvote", UserId = userId, EventId = Guid.NewGuid() };
        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        using var scope = ServiceProvider.CreateScope();
        var reputationEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();

        var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == userId);
        var initialReputationEarnedToday = user!.ReputationEarnedToday;

        //Act
        await reputationEventConsumer.Consume(contextMock.Object);

        //Assert
        Assert.NotNull(user);
        Assert.Equal(MockRepositoriesGetters.MinReputation, user.Reputation);
        Assert.Equal(initialReputationEarnedToday, user.ReputationEarnedToday);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeWrongEvent_ShouldBe_NoException()
    {
        //Arrange
        const long userId = 1;
        var message = new BaseEvent { EventType = "WrongEvent", UserId = userId, EventId = Guid.NewGuid() };
        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        using var scope = ServiceProvider.CreateScope();
        var reputationEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();

        var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == userId);
        var initialReputation = user!.Reputation;
        var initialReputationEarnedToday = user.ReputationEarnedToday;

        //Act
        await reputationEventConsumer.Consume(contextMock.Object);

        //Assert
        Assert.NotNull(user);
        Assert.Equal(initialReputation, user.Reputation);
        Assert.Equal(initialReputationEarnedToday, user.ReputationEarnedToday);
    }
}