using Microsoft.Extensions.Options;
using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Settings;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.Configurations;

namespace UserService.Tests.UnitTests.Factories;

internal class GetUserServiceFactory
{
    private readonly IGetUserService _getUserService;

    public readonly IBaseRepository<ReputationRecord> ReputationRecordRepository =
        MockRepositoriesGetters.GetMockReputationRecordRepository().Object;

    public readonly ReputationRules ReputationRules = ReputationRulesConfiguration.GetReputationRules();
    public readonly IBaseRepository<Role> RoleRepository = MockRepositoriesGetters.GetMockRoleRepository().Object;
    public readonly IBaseRepository<User> UserRepository = MockRepositoriesGetters.GetMockUserRepository().Object;

    public GetUserServiceFactory()
    {
        _getUserService = new GetUserService(UserRepository, RoleRepository, ReputationRecordRepository,
            Options.Create(ReputationRules));
    }

    public IGetUserService GetService()
    {
        return _getUserService;
    }
}