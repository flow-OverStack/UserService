using UserService.Application.Services;
using UserService.Domain.Dtos.Request.Page;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Interfaces.Validation;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.Configurations;

namespace UserService.Tests.UnitTests.Factories;

internal class GetRoleServiceFactory
{
    private readonly IGetRoleService _getRoleService;
    public readonly IFallbackValidator<PageDto> PageDtoValidator = PageDtoValidatorConfiguration.GetValidator();
    public readonly IBaseRepository<Role> RoleRepository = MockRepositoriesGetters.GetMockRoleRepository().Object;
    public readonly IBaseRepository<User> UserRepository = MockRepositoriesGetters.GetMockUserRepository().Object;

    public GetRoleServiceFactory(IBaseRepository<User>? userRepository = null,
        IBaseRepository<Role>? roleRepository = null)
    {
        if (userRepository != null)
            UserRepository = userRepository;

        if (roleRepository != null)
            RoleRepository = roleRepository;

        _getRoleService = new GetRoleService(UserRepository, RoleRepository, PageDtoValidator);
    }

    public IGetRoleService GetService()
    {
        return _getRoleService;
    }
}