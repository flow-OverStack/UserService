using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Repository;
using UserService.Messaging.Events;
using UserService.Tests.Configurations;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

[Collection(nameof(ReputationConsumerTests))]
public class ReputationConsumerTests(FunctionalTestWebAppFactory factory) : SequentialFunctionalTest(factory)
{
    public static TheoryData<(string EventName, string EntityType)> GetPositiveEvents()
    {
        return
        [
            (nameof(BaseEventType.AnswerAccepted), nameof(EntityType.Answer)),
            (nameof(BaseEventType.AnswerUpvote), nameof(EntityType.Answer)),
            (nameof(BaseEventType.QuestionUpvote), nameof(EntityType.Question)),
            (nameof(BaseEventType.UserAcceptedAnswer), nameof(EntityType.Answer))
        ];
    }

    public static TheoryData<(string EventName, string EntityType)> GetNegativeEvents()
    {
        return
        [
            (nameof(BaseEventType.AnswerDownvote), nameof(EntityType.Answer)),
            (nameof(BaseEventType.DownvoteGivenForAnswer), nameof(EntityType.Answer)),
            (nameof(BaseEventType.QuestionDownvote), nameof(EntityType.Question))
        ];
    }

    [Trait("Category", "Functional")]
    [Theory]
    [MemberData(nameof(GetPositiveEvents))]
    public async Task ConsumePositiveEvent_ShouldBe_Ok((string EventName, string EntityType) data)
    {
        //Arrange
        const long userId = 1;
        const long entityId = 2;

        var message = new BaseEvent
        {
            EventType = data.EventName,
            EntityType = data.EntityType,
            UserId = userId,
            EntityId = entityId,
            EventId = Guid.NewGuid()
        };

        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        await using var scope = ServiceProvider.CreateAsyncScope();
        var baseEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ReputationRecord>>();

        var initialReputation = await GetCurrentReputation(repository, userId);
        var initialRemainingReputation = await GetRemainingReputation(repository, userId);

        //Act
        await baseEventConsumer.Consume(contextMock.Object);

        //Assert
        var updatedReputation = await GetCurrentReputation(repository, userId);
        var updatedRemainingReputation = await GetRemainingReputation(repository, userId);

        Assert.True(updatedReputation > initialReputation);
        Assert.True(initialRemainingReputation >= updatedRemainingReputation);
    }

    [Trait("Category", "Functional")]
    [Theory]
    [MemberData(nameof(GetNegativeEvents))]
    public async Task ConsumeNegativeEvent_ShouldBe_Ok((string EventName, string EntityType) data)
    {
        //Arrange
        const long userId = 1;
        const long entityId = 1;

        var message = new BaseEvent
        {
            EventType = data.EventName,
            EntityType = data.EntityType,
            UserId = userId,
            EntityId = entityId,
            EventId = Guid.NewGuid()
        };

        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        await using var scope = ServiceProvider.CreateAsyncScope();
        var baseEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ReputationRecord>>();

        var initialReputation = await GetCurrentReputation(repository, userId);
        var initialRemainingReputation = await GetRemainingReputation(repository, userId);

        //Act
        await baseEventConsumer.Consume(contextMock.Object);

        //Assert
        var updatedReputation = await GetCurrentReputation(repository, userId);
        var updatedRemainingReputation = await GetRemainingReputation(repository, userId);

        Assert.True(updatedReputation < initialReputation);
        Assert.True(updatedRemainingReputation >= initialRemainingReputation);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumePositiveEvent_ShouldBe_DailyReputationLimitExceeded()
    {
        //Arrange
        const long userId = 3;
        const long entityId = 1;
        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.AnswerAccepted),
            EntityType = nameof(EntityType.Answer),
            UserId = userId,
            EntityId = entityId,
            EventId = Guid.NewGuid()
        };
        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        await using var scope = ServiceProvider.CreateAsyncScope();
        var baseEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ReputationRecord>>();

        var initialReputation = await GetCurrentReputation(repository, userId);

        //Act
        await baseEventConsumer.Consume(contextMock.Object);

        //Assert
        var updatedReputation = await GetCurrentReputation(repository, userId);
        var updatedRemainingReputation = await GetRemainingReputation(repository, userId);

        Assert.Equal(0, updatedRemainingReputation);
        Assert.Equal(initialReputation, updatedReputation);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeNegativeEvent_ShouldBe_ReputationMinimumReached()
    {
        //Arrange
        const long userId = 1;
        const long entityId = 1;
        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.AnswerDownvote),
            EntityType = nameof(EntityType.Answer),
            UserId = userId,
            EntityId = entityId,
            EventId = Guid.NewGuid()
        };
        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);


        await using var scope = ServiceProvider.CreateAsyncScope();
        var baseEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ReputationRecord>>();

        await DisableAllReputationRecords(repository, userId);

        var initialRemainingReputation = await GetRemainingReputation(repository, userId);

        //Act
        await baseEventConsumer.Consume(contextMock.Object);

        //Assert
        var updatedReputation = await GetCurrentReputation(repository, userId);
        var updatedRemainingReputation = await GetRemainingReputation(repository, userId);

        Assert.Equal(MockRepositoriesGetters.MinReputation, updatedReputation);
        Assert.Equal(initialRemainingReputation, updatedRemainingReputation);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeEntityDeletedEvent_ShouldBe_Ok()
    {
        //Arrange
        const long userId = 1;
        const long entityId = 3;
        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.EntityDeleted),
            EntityType = nameof(EntityType.Question),
            UserId = userId,
            EntityId = entityId,
            EventId = Guid.NewGuid()
        };
        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        await using var scope = ServiceProvider.CreateAsyncScope();
        var baseEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ReputationRecord>>();

        //Act
        await baseEventConsumer.Consume(contextMock.Object);

        //Assert
        var hasEnabledRecords = await repository.GetAll()
            .Include(x => x.ReputationRule)
            .AnyAsync(x =>
                x.EntityId == entityId &&
                x.ReputationRule.EntityType == nameof(EntityType.Question));
        Assert.False(hasEnabledRecords);
    }

    private async Task<int> GetCurrentReputation(IBaseRepository<ReputationRecord> repository, long userId)
    {
        if (!await repository.GetAll().AnyAsync(x => x.UserId == userId))
            return MockRepositoriesGetters.MinReputation;

        return await repository.GetAll()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Include(x => x.ReputationRule)
            .GroupBy(x => new { x.UserId, x.CreatedAt.Date })
            .Select(x => Math.Max(MockRepositoriesGetters.MinReputation,
                Math.Min(x.Sum(y => y.ReputationRule.ReputationChange),
                    MockRepositoriesGetters.MaxDailyReputation)))
            .FirstOrDefaultAsync();
    }

    private async Task<int> GetRemainingReputation(IBaseRepository<ReputationRecord> repository, long userId)
    {
        if (!await repository.GetAll().AnyAsync(x =>
                x.UserId == userId && x.CreatedAt.Date == DateTime.UtcNow.Date &&
                x.ReputationRule.ReputationChange > 0))
            return MockRepositoriesGetters.MaxDailyReputation;

        return await repository
            .GetAll().AsNoTracking()
            .Include(x => x.ReputationRule)
            .Where(x => x.UserId == userId && x.CreatedAt.Date == DateTime.UtcNow.Date &&
                        x.ReputationRule.ReputationChange > 0)
            .GroupBy(x => x.UserId)
            .Select(x => Math.Max(0,
                MockRepositoriesGetters.MaxDailyReputation - x.Sum(y => y.ReputationRule.ReputationChange)))
            .FirstOrDefaultAsync();
    }

    private static Task DisableAllReputationRecords(IBaseRepository<ReputationRecord> repository, long userId)
    {
        return repository.GetAll()
            .Where(x => x.UserId == userId)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.Enabled, p => false));
    }
}