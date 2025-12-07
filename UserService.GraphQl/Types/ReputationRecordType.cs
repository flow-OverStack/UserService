using UserService.Domain.Entities;
using UserService.GraphQl.DataLoaders;

namespace UserService.GraphQl.Types;

public class ReputationRecordType : ObjectType<ReputationRecord>
{
    protected override void Configure(IObjectTypeDescriptor<ReputationRecord> descriptor)
    {
        descriptor.Description("The reputation record type.");

        descriptor.Field(x => x.Id).Description("The ID of the reputation record.");
        descriptor.Field(x => x.UserId).Description("The ID of the user this record belongs to.");
        descriptor.Field(x => x.ReputationRuleId).Description("The ID of the reputation rule applied.");
        descriptor.Field(x => x.EntityId).Description("The ID of the related entity that triggered the rule.");
        descriptor.Field(x => x.Enabled).Ignore();
        descriptor.Field(x => x.CreatedAt).Description("The creation time of the record.");
        descriptor.Field(x => x.User).Description("The user associated with this reputation record.");
        descriptor.Field(x => x.ReputationRule).Description("The reputation rule that was applied for this record.");

        descriptor.Field(x => x.User).ResolveWith<Resolvers>(x => x.GetUserAsync(default!, default!, default!));
        descriptor.Field(x => x.ReputationRule)
            .ResolveWith<Resolvers>(x => x.GetReputationRuleAsync(default!, default!, default!));
    }

    private sealed class Resolvers
    {
        public async Task<User> GetUserAsync([Parent] ReputationRecord record, UserDataLoader userLoader,
            CancellationToken cancellationToken)
        {
            var user = await userLoader.LoadRequiredAsync(record.UserId, cancellationToken);
            return user;
        }

        public async Task<ReputationRule> GetReputationRuleAsync([Parent] ReputationRecord record,
            ReputationRuleDataLoader ruleLoader, CancellationToken cancellationToken)
        {
            var rule = await ruleLoader.LoadRequiredAsync(record.ReputationRuleId, cancellationToken);
            return rule;
        }
    }
}