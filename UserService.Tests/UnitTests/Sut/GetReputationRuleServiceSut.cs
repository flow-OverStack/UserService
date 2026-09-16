using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Mocks;

namespace UserService.Tests.UnitTests.Sut;

internal class GetReputationRuleServiceSut
{
    private readonly IGetReputationRuleService _getReputationRuleService;

    public readonly IBaseRepository<ReputationRule> ReputationRuleRepository =
        RepositoryMocks.GetMockReputationRuleRepository().Object;

    public GetReputationRuleServiceSut()
    {
        _getReputationRuleService = new GetReputationRuleService(ReputationRuleRepository);
    }

    public IGetReputationRuleService GetService()
    {
        return _getReputationRuleService;
    }
}
