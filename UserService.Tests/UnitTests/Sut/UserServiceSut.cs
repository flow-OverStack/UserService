using AutoMapper;
using FluentValidation;
using Hangfire;
using UserService.Application.Validators;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Mocks;
using UserService.Tests.UnitTests.Fixtures;

namespace UserService.Tests.UnitTests.Sut;

internal class UserServiceSut
{
    private readonly IUserService _userService;

    public readonly IBackgroundJobClient BackgroundJob =
        BackgroundJobClientFixture.GetBackgroundJobClientConfiguration();

    public readonly IIdentityServer IdentityServer = IdentityServerFixture.GetIdentityServerConfiguration();

    public readonly IMapper Mapper = MapperFixture.GetMapperConfiguration();

    public readonly IUnitOfWork UnitOfWork;

    public readonly IValidator<UpdateUsernameDto> UpdateUsernameValidator =
        ValidatorFixture<UpdateUsernameDto>.GetValidator(new UpdateUsernameDtoValidator());

    public UserServiceSut(IBaseRepository<User>? userRepository = null)
    {
        UnitOfWork = RepositoryMocks.GetMockUnitOfWork(userRepository).Object;

        _userService = new Application.Services.UserService(Mapper, IdentityServer, UnitOfWork, BackgroundJob,
            UpdateUsernameValidator);
    }

    public IUserService GetService()
    {
        return _userService;
    }
}
