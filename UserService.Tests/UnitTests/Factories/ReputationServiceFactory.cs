using UserService.Application.Services;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Settings;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.Configurations;

namespace UserService.Tests.UnitTests.Factories;

internal class ReputationServiceFactory
{
    private readonly IReputationService _reputationService;
    public readonly ReputationRules ReputationRules = ReputationRulesConfiguration.GetReputationRules();
    public readonly IUnitOfWork UnitOfWork = MockRepositoriesGetters.GetMockUnitOfWork().Object;

    public ReputationServiceFactory()
    {
        _reputationService = new ReputationService(UnitOfWork);
    }

    public IReputationService GetService()
    {
        return _reputationService;
    }
}