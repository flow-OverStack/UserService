using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Configurations;

namespace UserService.Tests.UnitTests.Factories;

internal class GetRoleServiceFactory
{
    private readonly IGetRoleService _getRoleService;
    public readonly IBaseRepository<Role> RoleRepository = MockRepositoriesGetters.GetMockRoleRepository().Object;

    public readonly IBaseRepository<User> UserRepository = MockRepositoriesGetters.GetMockUserRepository().Object;

    public GetRoleServiceFactory(IBaseRepository<User>? userRepository = null,
        IBaseRepository<Role>? roleRepository = null)
    {
        if (userRepository != null)
            UserRepository = userRepository;

        if (roleRepository != null)
            RoleRepository = roleRepository;

        _getRoleService = new GetRoleService(UserRepository, RoleRepository);
    }

    public IGetRoleService GetService()
    {
        return _getRoleService;
    }
}