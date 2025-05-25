using UserService.Domain.Settings;

namespace UserService.Tests.UnitTests.Configurations;

internal static class BusinessRulesConfiguration
{
    public static BusinessRules GetBusinessRules()
    {
        return new BusinessRules
        {
            MaxDailyReputation = 200,
            MinReputation = 1,
            MaxPageSize = 100
        };
    }
}