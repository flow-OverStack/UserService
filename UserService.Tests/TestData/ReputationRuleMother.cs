using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Settings;

namespace UserService.Tests.TestData;

internal static class ReputationRuleMother
{
    public static IQueryable<ReputationRule> GetReputationRules()
    {
        return new[]
        {
            new ReputationRule
            {
                Id = 1, EventType = nameof(BaseEventType.EntityAccepted), EntityType = nameof(EntityType.Answer),
                Group = null, ReputationChange = 15, ReputationTarget = ReputationTarget.Author
            },
            new ReputationRule
            {
                Id = 2, EventType = nameof(BaseEventType.EntityDownvoted),
                ReputationTarget = ReputationTarget.Initiator, EntityType = nameof(EntityType.Answer), Group = null,
                ReputationChange = -1
            },
            new ReputationRule
            {
                Id = 3, EventType = nameof(BaseEventType.EntityDownvoted), EntityType = nameof(EntityType.Answer),
                Group = "Vote", ReputationChange = -2, ReputationTarget = ReputationTarget.Author
            },
            new ReputationRule
            {
                Id = 4, EventType = nameof(BaseEventType.EntityUpvoted), EntityType = nameof(EntityType.Answer),
                Group = "Vote", ReputationChange = 10, ReputationTarget = ReputationTarget.Author
            },
            new ReputationRule
            {
                Id = 5, EventType = nameof(BaseEventType.EntityAccepted), EntityType = nameof(EntityType.Answer),
                Group = null, ReputationChange = 2, ReputationTarget = ReputationTarget.Initiator
            },
            new ReputationRule
            {
                Id = 6, EventType = nameof(BaseEventType.EntityDownvoted), EntityType = nameof(EntityType.Question),
                Group = "Vote", ReputationChange = -2, ReputationTarget = ReputationTarget.Author
            },
            new ReputationRule
            {
                Id = 7, EventType = nameof(BaseEventType.EntityUpvoted), EntityType = nameof(EntityType.Question),
                Group = "Vote", ReputationChange = 10, ReputationTarget = ReputationTarget.Author
            },
            new ReputationRule
            {
                Id = 8, EventType = "TestSuperEvent", EntityType = nameof(EntityType.Question),
                Group = null, ReputationChange = BusinessRules.MaxDailyReputation,
                ReputationTarget = ReputationTarget.Author
            }
        }.AsQueryable();
    }
}
