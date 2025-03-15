using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using UserService.DAL.Repositories;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Repositories;
using UserService.Tests.Configurations;

namespace UserService.Tests.FunctionalTests.Base.Exception.ReputationResetJob;

public class ReputationResetJobExceptionFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private static Mock<IBaseRepository<User>> GetExceptionMockUserRepository(
        IBaseRepository<User> originalUserRepository)
    {
        var mockRepository = new Mock<IBaseRepository<User>>();

        mockRepository.Setup(x => x.GetAll()).Throws(new TestException());
        mockRepository.Setup(x => x.Remove(It.IsAny<User>())).Returns(originalUserRepository.Remove);
        mockRepository.Setup(x => x.Update(It.IsAny<User>())).Returns(originalUserRepository.Update);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<User>())).Returns(originalUserRepository.CreateAsync);

        return mockRepository;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IBaseRepository<User>>();
            services.AddScoped<IBaseRepository<User>>(provider =>
            {
                //the dependencies from service provider only apply for this current scope
                //that is why we have to use ActivatorUtilities to transfer dependencies from this scope to callers' scope
                var userRepository = ActivatorUtilities.CreateInstance<BaseRepository<User>>(provider);
                var exceptionUserRepository = GetExceptionMockUserRepository(userRepository).Object;

                return exceptionUserRepository;
            });
        });
    }
}