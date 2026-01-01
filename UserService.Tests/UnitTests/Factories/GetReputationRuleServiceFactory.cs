using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Configurations;

namespace UserService.Tests.UnitTests.Factories;

internal class GetReputationRuleServiceFactory
{
    private readonly IGetReputationRuleService _getReputationRuleService;

    public readonly IBaseRepository<ReputationRule> ReputationRuleRepository =
        MockRepositoriesGetters.GetMockReputationRuleRepository().Object;

    public GetReputationRuleServiceFactory()
    {
        _getReputationRuleService = new GetReputationRuleService(ReputationRuleRepository);
    }

    public IGetReputationRuleService GetService()
    {
        return _getReputationRuleService;
    }
}