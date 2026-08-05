using UserService.Application.Services.Provisioning;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Tests.Configurations;

namespace UserService.Tests.UnitTests.Factories;

internal class UsernameGeneratorFactory
{
    private readonly IUsernameGenerator _usernameGenerator;

    public readonly IBaseRepository<User> UserRepository = MockRepositoriesGetters.GetMockUserRepository().Object;

    public UsernameGeneratorFactory()
    {
        _usernameGenerator = new UsernameGenerator(UserRepository);
    }

    public IUsernameGenerator GetService()
    {
        return _usernameGenerator;
    }
}
