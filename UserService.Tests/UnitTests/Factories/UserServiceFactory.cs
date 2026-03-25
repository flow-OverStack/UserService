using AutoMapper;
using FluentValidation;
using Hangfire;
using UserService.Application.Validators;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.Configurations;
using MapperConfiguration = UserService.Tests.UnitTests.Configurations.MapperConfiguration;

namespace UserService.Tests.UnitTests.Factories;

internal class UserServiceFactory
{
    private readonly IUserService _userService;

    public readonly IBackgroundJobClient BackgroundJob =
        BackgroundJobClientConfiguration.GetBackgroundJobClientConfiguration();

    public readonly IIdentityServer IdentityServer = IdentityServerConfiguration.GetIdentityServerConfiguration();

    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();

    public readonly IUnitOfWork UnitOfWork;

    public readonly IValidator<UpdateUsernameDto> UpdateUsernameValidator =
        ValidatorConfiguration<UpdateUsernameDto>.GetValidator(new UpdateUsernameDtoValidator());

    public UserServiceFactory(IBaseRepository<User>? userRepository = null)
    {
        UnitOfWork = MockRepositoriesGetters.GetMockUnitOfWork(userRepository).Object;

        _userService = new Application.Services.UserService(Mapper, IdentityServer, UnitOfWork, BackgroundJob,
            UpdateUsernameValidator);
    }

    public IUserService GetService()
    {
        return _userService;
    }
}