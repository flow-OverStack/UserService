using AutoMapper;
using FluentValidation;
using Moq;
using UserService.Application.Services;
using UserService.Application.Services.Provisioning;
using UserService.Application.Validators;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.Configurations;
using MapperConfiguration = UserService.Tests.UnitTests.Configurations.MapperConfiguration;

namespace UserService.Tests.UnitTests.Factories;

internal class AuthServiceFactory
{
    private readonly IAuthService _authService;

    public readonly Mock<IIdentityCompensationQueue> CompensationQueue =
        IdentityCompensationQueueConfiguration.GetMockIdentityCompensationQueue();

    public readonly IIdentityServer IdentityServer = IdentityServerConfiguration.GetIdentityServerConfiguration();

    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();

    public readonly Mock<IUserProvisioningService> ProvisioningService = new();

    public readonly IValidator<RegisterUserDto> RegisterValidator =
        ValidatorConfiguration<RegisterUserDto>.GetValidator(new RegisterUserDtoValidator());

    public readonly IUnitOfWork UnitOfWork;

    public readonly Mock<IUserSyncQueue> UserSyncQueue = UserSyncQueueConfiguration.GetMockUserSyncQueue();

    public AuthServiceFactory(IBaseRepository<User>? userRepository = null,
        IBaseRepository<Role>? roleRepository = null)
    {
        UnitOfWork = MockRepositoriesGetters.GetMockUnitOfWork(userRepository, roleRepository).Object;

        _authService = new AuthService(Mapper, IdentityServer, UnitOfWork, CompensationQueue.Object,
            UserSyncQueue.Object, ProvisioningService.Object, RegisterValidator);
    }

    public IAuthService GetService()
    {
        return _authService;
    }
}
