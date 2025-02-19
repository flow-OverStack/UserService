using UserService.Application.Services;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Tests.Configurations;

namespace UserService.Tests.UnitTests.ServiceFactories;

public class ReputationResetServiceFactory
{
    private readonly IReputationResetService _reputationService;
    public readonly IBaseRepository<User> UserRepository = MockRepositoriesGetters.GetMockUserRepository().Object;

    public ReputationResetServiceFactory()
    {
        _reputationService = new ReputationService(UserRepository);
    }

    public IReputationResetService GetService()
    {
        return _reputationService;
    }
}