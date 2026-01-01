using UserService.Domain.Settings;
using UserService.Tests.Configurations;

namespace UserService.Tests.UnitTests.Configurations;

internal static class ReputationRulesConfiguration
{
    public static ReputationRules GetReputationRules()
    {
        return new ReputationRules
        {
            MinReputation = MockRepositoriesGetters.MinReputation,
            MaxDailyReputation = MockRepositoriesGetters.MaxDailyReputation
        };
    }
}