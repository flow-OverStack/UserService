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
    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeQuestionUpvoted_ShouldBe_Ok()
    {
        //Arrange
        const long authorId = 2;
        const long entityId = 1;
        const long initiatorId = 1;

        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.EntityUpvoted),
            EntityType = nameof(EntityType.Question),
            AuthorId = authorId,
            InitiatorId = initiatorId,
            EntityId = entityId,
            EventId = Guid.NewGuid()
        };

        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        await using var scope = ServiceProvider.CreateAsyncScope();
        var baseEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ReputationRecord>>();

        var initialReputation = await GetCurrentReputation(repository, authorId);
        var initialRemainingReputation = await GetRemainingReputation(repository, authorId);

        //Act
        await baseEventConsumer.Consume(contextMock.Object);

        //Assert
        var updatedReputation = await GetCurrentReputation(repository, authorId);
        var updatedRemainingReputation = await GetRemainingReputation(repository, authorId);
        var disabledRecordsCount = await repository.GetAll().IgnoreQueryFilters()
            .CountAsync(x => x.InitiatorId == initiatorId && !x.Enabled);
        var count = await repository.GetAll().CountAsync();

        Assert.True(updatedReputation > initialReputation);
        Assert.True(initialRemainingReputation >= updatedRemainingReputation);
        Assert.Equal(1, disabledRecordsCount); // Should be 1 disabled record
        Assert.Equal(9, count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeAnswerUpvoted_ShouldBe_Ok()
    {
        //Arrange
        const long authorId = 2;
        const long entityId = 1;
        const long initiatorId = 1;

        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.EntityUpvoted),
            EntityType = nameof(EntityType.Answer),
            AuthorId = authorId,
            InitiatorId = initiatorId,
            EntityId = entityId,
            EventId = Guid.NewGuid()
        };

        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        await using var scope = ServiceProvider.CreateAsyncScope();
        var baseEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ReputationRecord>>();

        var initialReputation = await GetCurrentReputation(repository, authorId);
        var initialRemainingReputation = await GetRemainingReputation(repository, authorId);

        //Act
        await baseEventConsumer.Consume(contextMock.Object);

        //Assert
        var updatedReputation = await GetCurrentReputation(repository, authorId);
        var updatedRemainingReputation = await GetRemainingReputation(repository, authorId);
        var disabledRecordsCount = await repository.GetAll().IgnoreQueryFilters()
            .CountAsync(x => x.InitiatorId == initiatorId && !x.Enabled);
        var count = await repository.GetAll().CountAsync();

        Assert.True(updatedReputation > initialReputation);
        Assert.True(initialRemainingReputation >= updatedRemainingReputation);
        Assert.Equal(2, disabledRecordsCount); // Should be 2 disabled records
        Assert.Equal(8, count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeAnswerAccepted_ShouldBe_Ok()
    {
        //Arrange
        const long authorId = 2;
        const long entityId = 1;
        const long initiatorId = 1;

        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.EntityAccepted),
            EntityType = nameof(EntityType.Answer),
            AuthorId = authorId,
            InitiatorId = initiatorId,
            EntityId = entityId,
            EventId = Guid.NewGuid()
        };

        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        await using var scope = ServiceProvider.CreateAsyncScope();
        var baseEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ReputationRecord>>();

        var initialReputation = await GetCurrentReputation(repository, authorId);
        var initialRemainingReputation = await GetRemainingReputation(repository, authorId);

        //Act
        await baseEventConsumer.Consume(contextMock.Object);

        //Assert
        var updatedReputation = await GetCurrentReputation(repository, authorId);
        var updatedRemainingReputation = await GetRemainingReputation(repository, authorId);
        var disabledRecordsCount = await repository.GetAll().IgnoreQueryFilters()
            .CountAsync(x => x.InitiatorId == initiatorId && !x.Enabled);
        var count = await repository.GetAll().CountAsync();

        Assert.True(updatedReputation > initialReputation);
        Assert.True(initialRemainingReputation >= updatedRemainingReputation);
        Assert.Equal(0, disabledRecordsCount); // Should be 0 disabled record
        Assert.Equal(11, count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeQuestionDownvoted_ShouldBe_Ok()
    {
        //Arrange
        const long authorId = 1;
        const long entityId = 2;
        const long initiatorId = 2;

        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.EntityDownvoted),
            EntityType = nameof(EntityType.Question),
            AuthorId = authorId,
            InitiatorId = initiatorId,
            EntityId = entityId,
            EventId = Guid.NewGuid()
        };

        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        await using var scope = ServiceProvider.CreateAsyncScope();
        var baseEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ReputationRecord>>();

        var initialReputation = await GetCurrentReputation(repository, authorId);
        var initialRemainingReputation = await GetRemainingReputation(repository, authorId);

        //Act
        await baseEventConsumer.Consume(contextMock.Object);

        //Assert
        var updatedReputation = await GetCurrentReputation(repository, authorId);
        var updatedRemainingReputation = await GetRemainingReputation(repository, authorId);
        var disabledRecordsCount = await repository.GetAll().IgnoreQueryFilters()
            .CountAsync(x => x.InitiatorId == initiatorId && !x.Enabled);
        var count = await repository.GetAll().CountAsync();

        Assert.True(updatedReputation < initialReputation);
        Assert.True(updatedRemainingReputation >= initialRemainingReputation);
        Assert.Equal(1, disabledRecordsCount); // Should be 1 disabled record
        Assert.Equal(9, count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeAnswerDownvoted_ShouldBe_Ok()
    {
        //Arrange
        const long authorId = 1;
        const long entityId = 2;
        const long initiatorId = 2;

        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.EntityDownvoted),
            EntityType = nameof(EntityType.Answer),
            AuthorId = authorId,
            InitiatorId = initiatorId,
            EntityId = entityId,
            EventId = Guid.NewGuid()
        };

        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        await using var scope = ServiceProvider.CreateAsyncScope();
        var baseEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ReputationRecord>>();

        var initialReputation = await GetCurrentReputation(repository, authorId);
        var initialRemainingReputation = await GetRemainingReputation(repository, authorId);

        //Act
        await baseEventConsumer.Consume(contextMock.Object);

        //Assert
        var updatedReputation = await GetCurrentReputation(repository, authorId);
        var updatedRemainingReputation = await GetRemainingReputation(repository, authorId);
        var disabledRecordsCount = await repository.GetAll().IgnoreQueryFilters()
            .CountAsync(x => x.InitiatorId == initiatorId && !x.Enabled);
        var count = await repository.GetAll().CountAsync();

        Assert.True(updatedReputation < initialReputation);
        Assert.True(updatedRemainingReputation >= initialRemainingReputation);
        Assert.Equal(1, disabledRecordsCount); // Should be 1 disabled record
        Assert.Equal(10, count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeEntityAcceptanceRevoked_ShouldBe_Ok()
    {
        //Arrange
        const long authorId = 1;
        const long entityId = 2;
        const long initiatorId = 2;

        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.EntityAcceptanceRevoked),
            EntityType = nameof(EntityType.Answer),
            AuthorId = authorId,
            InitiatorId = initiatorId,
            EntityId = entityId,
            EventId = Guid.NewGuid()
        };

        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        await using var scope = ServiceProvider.CreateAsyncScope();
        var baseEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ReputationRecord>>();

        var initialReputation = await GetCurrentReputation(repository, authorId);
        var initialRemainingReputation = await GetRemainingReputation(repository, authorId);

        //Act
        await baseEventConsumer.Consume(contextMock.Object);

        //Assert
        var updatedReputation = await GetCurrentReputation(repository, authorId);
        var updatedRemainingReputation = await GetRemainingReputation(repository, authorId);
        var disabledRecordsCount = await repository.GetAll().IgnoreQueryFilters().AsNoTracking()
            .CountAsync(x => x.InitiatorId == initiatorId && !x.Enabled);
        var count = await repository.GetAll().CountAsync();

        Assert.True(updatedReputation < initialReputation);
        Assert.True(updatedRemainingReputation >= initialRemainingReputation);
        Assert.Equal(2, disabledRecordsCount); // Should be 2 disabled records
        Assert.Equal(7, count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeEntityDeletedEvent_ShouldBe_Ok()
    {
        //Arrange
        const long authorId = 1;
        const long entityId = 1;
        const long initiatorId = 2;

        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.EntityDeleted),
            EntityType = nameof(EntityType.Question),
            AuthorId = authorId,
            InitiatorId = initiatorId,
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

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeEntityVoteRemoved_ShouldBe_Ok()
    {
        // Arrange
        const long authorId = 2;
        const long initiatorId = 1;
        const long entityId = 1;

        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.EntityVoteRemoved),
            EntityType = nameof(EntityType.Question),
            AuthorId = authorId,
            InitiatorId = initiatorId,
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
        var hasVotes = await repository.GetAll()
            .Include(x => x.ReputationRule)
            .AnyAsync(x => x.InitiatorId == initiatorId
                           && x.ReputationRule.EntityType == nameof(EntityType.Question)
                           && x.EntityId == entityId
                           && (x.ReputationRule.EventType ==
                               nameof(BaseEventType.EntityUpvoted) ||
                               x.ReputationRule.EventType ==
                               nameof(BaseEventType.EntityDownvoted)));
        Assert.False(hasVotes);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumePositiveEvent_ShouldBe_DailyReputationLimitExceeded()
    {
        //Arrange
        const long authorId = 3;
        const long entityId = 1;
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
        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);

        await using var scope = ServiceProvider.CreateAsyncScope();
        var baseEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ReputationRecord>>();

        var initialReputation = await GetCurrentReputation(repository, authorId);

        //Act
        await baseEventConsumer.Consume(contextMock.Object);

        //Assert
        var updatedReputation = await GetCurrentReputation(repository, authorId);
        var updatedRemainingReputation = await GetRemainingReputation(repository, authorId);

        Assert.Equal(0, updatedRemainingReputation);
        Assert.Equal(initialReputation, updatedReputation);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task ConsumeNegativeEvent_ShouldBe_ReputationMinimumReached()
    {
        //Arrange
        const long authorId = 1;
        const long entityId = 1;
        var message = new BaseEvent
        {
            EventType = nameof(BaseEventType.EntityDownvoted),
            EntityType = nameof(EntityType.Answer),
            AuthorId = authorId,
            EntityId = entityId,
            EventId = Guid.NewGuid()
        };
        var contextMock = new Mock<ConsumeContext<BaseEvent>>();
        contextMock.Setup(x => x.Message).Returns(message);


        await using var scope = ServiceProvider.CreateAsyncScope();
        var baseEventConsumer = scope.ServiceProvider.GetRequiredService<IConsumer<BaseEvent>>();
        var repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ReputationRecord>>();

        await DisableAllReputationRecords(repository, authorId);

        var initialRemainingReputation = await GetRemainingReputation(repository, authorId);

        //Act
        await baseEventConsumer.Consume(contextMock.Object);

        //Assert
        var updatedReputation = await GetCurrentReputation(repository, authorId);
        var updatedRemainingReputation = await GetRemainingReputation(repository, authorId);

        Assert.Equal(MockRepositoriesGetters.MinReputation, updatedReputation);
        Assert.Equal(initialRemainingReputation, updatedRemainingReputation);
    }

    private async Task<int> GetCurrentReputation(IBaseRepository<ReputationRecord> repository, long userId)
    {
        if (!await repository.GetAll().AnyAsync(x => x.ReputationTargetId == userId))
            return MockRepositoriesGetters.MinReputation;

        return await repository.GetAll()
            .AsNoTracking()
            .Where(x => x.ReputationTargetId == userId)
            .Include(x => x.ReputationRule)
            .GroupBy(x => new { UserId = x.ReputationTargetId, x.CreatedAt.Date })
            .Select(x => Math.Max(MockRepositoriesGetters.MinReputation,
                Math.Min(x.Sum(y => y.ReputationRule.ReputationChange),
                    MockRepositoriesGetters.MaxDailyReputation)))
            .FirstOrDefaultAsync();
    }

    private static async Task<int> GetRemainingReputation(IBaseRepository<ReputationRecord> repository, long userId)
    {
        if (!await repository.GetAll().AnyAsync(x =>
                x.ReputationTargetId == userId && x.CreatedAt.Date == DateTime.UtcNow.Date &&
                x.ReputationRule.ReputationChange > 0))
            return MockRepositoriesGetters.MaxDailyReputation;

        return await repository
            .GetAll().AsNoTracking()
            .Include(x => x.ReputationRule)
            .Where(x => x.ReputationTargetId == userId && x.CreatedAt.Date == DateTime.UtcNow.Date &&
                        x.ReputationRule.ReputationChange > 0)
            .GroupBy(x => x.ReputationTargetId)
            .Select(x => Math.Max(0,
                MockRepositoriesGetters.MaxDailyReputation - x.Sum(y => y.ReputationRule.ReputationChange)))
            .FirstOrDefaultAsync();
    }

    private static Task<int> DisableAllReputationRecords(IBaseRepository<ReputationRecord> repository, long userId)
    {
        return repository.GetAll()
            .Where(x => x.ReputationTargetId == userId)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.Enabled, p => false));
    }
}