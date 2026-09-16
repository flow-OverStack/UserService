using UserService.Application.Services;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Mocks;

namespace UserService.Tests.UnitTests.Sut;

internal class ReputationServiceSut
{
    private readonly IReputationService _reputationService;
    public readonly IUnitOfWork UnitOfWork = RepositoryMocks.GetMockUnitOfWork().Object;

    public ReputationServiceSut()
    {
        _reputationService = new ReputationService(UnitOfWork);
    }

    public IReputationService GetService()
    {
        return _reputationService;
    }
}
