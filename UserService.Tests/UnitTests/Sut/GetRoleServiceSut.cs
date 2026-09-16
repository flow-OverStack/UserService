using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Mocks;

namespace UserService.Tests.UnitTests.Sut;

internal class GetRoleServiceSut
{
    private readonly IGetRoleService _getRoleService;
    public readonly IBaseRepository<Role> RoleRepository = RepositoryMocks.GetMockRoleRepository().Object;
    public readonly IBaseRepository<User> UserRepository = RepositoryMocks.GetMockUserRepository().Object;

    public GetRoleServiceSut(IBaseRepository<Role>? roleRepository = null)
    {
        if (roleRepository != null)
            RoleRepository = roleRepository;

        _getRoleService = new GetRoleService(UserRepository, RoleRepository);
    }

    public IGetRoleService GetService()
    {
        return _getRoleService;
    }
}
