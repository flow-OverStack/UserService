using UserService.Application.Services;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Tests.Configurations;

namespace UserService.Tests.UnitTests.ServiceFactories;

public class GraphQlServiceFactory
{
    private readonly IGraphQlService _graphQlService;
    public readonly IBaseRepository<Role> RoleRepository = MockRepositoriesGetters.GetMockRoleRepository().Object;

    public readonly IBaseRepository<User> UserRepository = MockRepositoriesGetters.GetMockUserRepository().Object;

    public GraphQlServiceFactory(IBaseRepository<User>? userRepository = null,
        IBaseRepository<Role>? roleRepository = null)
    {
        if (userRepository != null)
            UserRepository = userRepository;

        if (roleRepository != null)
            RoleRepository = roleRepository;

        _graphQlService = new GraphQlService(UserRepository, RoleRepository);
    }

    public IGraphQlService GetService()
    {
        return _graphQlService;
    }
}