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

public class KafkaConsumerTests(KafkaConsumerFunctionalTestWebAppFactory factory)
    : KafkaConsumerBaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeAnswerAccepted_ShouldBe_Success()
    {
        //Arrange
        const long userId = 1;
        var message = new BaseEvent { EventType = "AnswerAccepted", UserId = userId };
        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        using var scope = ServiceProvider.CreateScope();
        var reputationEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();

        //Act
        await reputationEventConsumer.Consume(contextMock.Object);

        //Assert
        var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == userId);
        Assert.NotNull(user);
        Assert.True(user.Reputation > User.MinReputation);
        Assert.True(user.ReputationEarnedToday > 0);
    }
}