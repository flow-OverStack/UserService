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

    public readonly IIdentityCompensationQueue CompensationQueue = new Mock<IIdentityCompensationQueue>().Object;

    public readonly IIdentityServer IdentityServer = IdentityServerFixture.GetIdentityServerConfiguration();

    public readonly IMapper Mapper = MapperFixture.GetMapperConfiguration();

    public readonly IUserProvisioningService ProvisioningService = new Mock<IUserProvisioningService>().Object;

    public readonly IValidator<RegisterUserDto> RegisterValidator =
        ValidatorFixture<RegisterUserDto>.GetValidator(new RegisterUserDtoValidator());

    public readonly IUnitOfWork UnitOfWork;

    public readonly IUserSyncQueue UserSyncQueue = new Mock<IUserSyncQueue>().Object;

    public AuthServiceSut(IBaseRepository<User>? userRepository = null,
        IBaseRepository<Role>? roleRepository = null)
    {
        UnitOfWork = RepositoryMocks.GetMockUnitOfWork(userRepository, roleRepository).Object;

        _authService = new AuthService(Mapper, IdentityServer, UnitOfWork, CompensationQueue, UserSyncQueue,
            ProvisioningService, RegisterValidator);
    }

    public IAuthService GetService()
    {
        return _authService;
    }
}