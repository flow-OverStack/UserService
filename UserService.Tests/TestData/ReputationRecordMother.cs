using UserService.Domain.Entities;

namespace UserService.Tests.TestData;

internal static class ReputationRecordMother
{
    public static IQueryable<ReputationRecord> GetReputationRecords()
    {
        var rules = ReputationRuleMother.GetReputationRules().ToList();

        var ruleAnswerAccepted = rules.First(x => x.Id == 1);
        var ruleAnswerDownvoteInitiator = rules.First(x => x.Id == 2);
        var ruleAnswerDownvote = rules.First(x => x.Id == 3);
        var ruleAnswerUpvote = rules.First(x => x.Id == 4);
        var ruleAnswerAcceptedInitiator = rules.First(x => x.Id == 5);
        var ruleQuestionDownvote = rules.First(x => x.Id == 6);
        var ruleQuestionUpvote = rules.First(x => x.Id == 7);
        var superRule = rules.First(x => x.Id == 8);

        return new[]
        {
            new ReputationRecord
            {
                Id = 1,
                ReputationTargetId = 2,
                InitiatorId = 1,
                EntityId = 1,
                ReputationRule = ruleQuestionDownvote,
                ReputationRuleId = ruleQuestionDownvote.Id,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new ReputationRecord
            {
                Id = 2,
                ReputationTargetId = 3,
                InitiatorId = 1,
                EntityId = 1,
                ReputationRule = ruleAnswerDownvote,
                ReputationRuleId = ruleAnswerDownvote.Id,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new ReputationRecord
            {
                Id = 3,
                ReputationTargetId = 1,
                InitiatorId = 1,
                EntityId = 1,
                ReputationRule = ruleAnswerDownvoteInitiator,
                ReputationRuleId = ruleAnswerDownvoteInitiator.Id,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new ReputationRecord
            {
                Id = 4,
                ReputationTargetId = 3,
                InitiatorId = 2,
                EntityId = 2,
                ReputationRule = ruleQuestionUpvote,
                ReputationRuleId = ruleQuestionUpvote.Id,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new ReputationRecord
            {
                Id = 5,
                ReputationTargetId = 1,
                InitiatorId = 2,
                EntityId = 2,
                ReputationRule = ruleAnswerUpvote,
                ReputationRuleId = ruleAnswerUpvote.Id,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new ReputationRecord
            {
                Id = 6,
                ReputationTargetId = 1,
                InitiatorId = 2,
                EntityId = 2,
                ReputationRule = ruleAnswerAccepted,
                ReputationRuleId = ruleAnswerAccepted.Id,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new ReputationRecord
            {
                Id = 7,
                ReputationTargetId = 2,
                InitiatorId = 2,
                EntityId = 2,
                ReputationRule = ruleAnswerAcceptedInitiator,
                ReputationRuleId = ruleAnswerAcceptedInitiator.Id,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new ReputationRecord
            {
                Id = 8,
                ReputationTargetId = 2,
                InitiatorId = 3,
                EntityId = 3,
                ReputationRuleId = ruleAnswerUpvote.Id,
                ReputationRule = ruleAnswerUpvote,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new ReputationRecord
            {
                Id = 9,
                ReputationTargetId = 3,
                InitiatorId = 3,
                EntityId = 3,
                ReputationRuleId = superRule.Id,
                ReputationRule = superRule,
                Enabled = true,
                CreatedAt = DateTime.UtcNow
            },
            new ReputationRecord
            {
                Id = 10,
                ReputationTargetId = 2,
                InitiatorId = 3,
                EntityId = 4,
                ReputationRuleId = ruleAnswerUpvote.Id,
                ReputationRule = ruleAnswerUpvote,
                Enabled = false,
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            }
        }.AsQueryable();
    }
}
