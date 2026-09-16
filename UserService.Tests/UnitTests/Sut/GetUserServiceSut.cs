using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Mocks;

namespace UserService.Tests.UnitTests.Sut;

internal class GetUserServiceSut
{
    private readonly IGetUserService _getUserService;

    public readonly IBaseRepository<ReputationRecord> ReputationRecordRepository =
        RepositoryMocks.GetMockReputationRecordRepository().Object;

    public readonly IBaseRepository<Role> RoleRepository = RepositoryMocks.GetMockRoleRepository().Object;
    public readonly IBaseRepository<User> UserRepository = RepositoryMocks.GetMockUserRepository().Object;

    public GetUserServiceSut()
    {
        _getUserService = new GetUserService(UserRepository, RoleRepository, ReputationRecordRepository);
    }

    public IGetUserService GetService()
    {
        return _getUserService;
    }
}
