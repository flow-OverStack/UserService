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
using UserService.Tests.Mocks;
using UserService.Tests.UnitTests.Fixtures;

namespace UserService.Tests.UnitTests.Sut;

internal class AuthServiceSut
{
    private readonly IAuthService _authService;

    public readonly Mock<IIdentityCompensationQueue> CompensationQueue =
        IdentityCompensationQueueFixture.GetMockIdentityCompensationQueue();

    public readonly IIdentityServer IdentityServer = IdentityServerFixture.GetIdentityServerConfiguration();

    public readonly IMapper Mapper = MapperFixture.GetMapperConfiguration();

    public readonly Mock<IUserProvisioningService> ProvisioningService = new();

    public readonly IValidator<RegisterUserDto> RegisterValidator =
        ValidatorFixture<RegisterUserDto>.GetValidator(new RegisterUserDtoValidator());

    public readonly IUnitOfWork UnitOfWork;

    public readonly Mock<IUserSyncQueue> UserSyncQueue = UserSyncQueueFixture.GetMockUserSyncQueue();

    public AuthServiceSut(IBaseRepository<User>? userRepository = null,
        IBaseRepository<Role>? roleRepository = null)
    {
        UnitOfWork = RepositoryMocks.GetMockUnitOfWork(userRepository, roleRepository).Object;

        _authService = new AuthService(Mapper, IdentityServer, UnitOfWork, CompensationQueue.Object,
            UserSyncQueue.Object, ProvisioningService.Object, RegisterValidator);
    }

    public IAuthService GetService()
    {
        return _authService;
    }
}
