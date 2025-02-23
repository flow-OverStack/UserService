using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Repositories;
using UserService.ReputationConsumer.Events;
using UserService.Tests.FunctionalTests.Base.Kafka;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class ReputationConsumerTests(ReputationConsumerFunctionalTestWebAppFactory factory)
    : ReputationConsumerBaseFunctionalTest(factory)
{
    public static TheoryData<string> GetPositiveEvents()
    {
        return ["AnswerAccepted", "AnswerUpvote", "QuestionUpvote", "UserAcceptedAnswer"];
    }

    public static TheoryData<string> GetNegativeEvents()
    {
        return ["AnswerDownvote", "DownvoteGiven", "QuestionDownvote"];
    }

    [Trait("Category", "Functional")]
    [Theory]
    [MemberData(nameof(GetPositiveEvents))]
    public async Task ConsumePositiveEvent_ShouldBe_Success(string eventName)
    {
        //Arrange
        const long userId = 1;
        var message = new BaseEvent { EventType = eventName, UserId = userId };
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
        var message = new BaseEvent { EventType = eventName, UserId = userId };
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
        Assert.Equal(user.ReputationEarnedToday, initialReputationEarnedToday);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumePositiveEvent_ShouldBe_DailyReputationLimitExceeded()
    {
        //Arrange
        const long userId = 3;
        var message = new BaseEvent { EventType = "AnswerAccepted", UserId = userId };
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
        Assert.Equal(User.MaxDailyReputation, user.ReputationEarnedToday);
        Assert.Equal(initialReputation, user.Reputation);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeNegativeEvent_ShouldBe_ReputationMinimumReached()
    {
        //Arrange
        const long userId = 1;
        var message = new BaseEvent { EventType = "AnswerDownvote", UserId = userId };
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
        Assert.Equal(User.MinReputation, user.Reputation);
        Assert.Equal(initialReputationEarnedToday, user.ReputationEarnedToday);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeWrongEvent_ShouldBe_NoException()
    {
        //Arrange
        const long userId = 1;
        var message = new BaseEvent { EventType = "WrongEvent", UserId = userId };
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