using UserService.Application.Services.Provisioning;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Tests.Mocks;

namespace UserService.Tests.UnitTests.Sut;

internal class UsernameGeneratorSut
{
    private readonly IUsernameGenerator _usernameGenerator;

    public readonly IBaseRepository<User> UserRepository = RepositoryMocks.GetMockUserRepository().Object;

    public UsernameGeneratorSut()
    {
        _usernameGenerator = new UsernameGenerator(UserRepository);
    }

    public IUsernameGenerator GetService()
    {
        return _usernameGenerator;
    }
}
