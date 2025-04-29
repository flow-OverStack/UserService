using Microsoft.Extensions.Options;
using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Settings;
using UserService.Tests.Configurations;

namespace UserService.Tests.UnitTests.Factories;

internal class ReputationServiceFactory
{
    private readonly BusinessRules _businessRules = new()
    {
        MinReputation = MockRepositoriesGetters.MinReputation,
        MaxDailyReputation = MockRepositoriesGetters.MaxDailyReputation
    };

    private readonly IReputationService _reputationService;
    public readonly IBaseRepository<User> UserRepository = MockRepositoriesGetters.GetMockUserRepository().Object;

    public ReputationServiceFactory()
    {
        _reputationService = new ReputationService(UserRepository, new OptionsWrapper<BusinessRules>(_businessRules));
    }

    public IReputationService GetService()
    {
        return _reputationService;
    }
}