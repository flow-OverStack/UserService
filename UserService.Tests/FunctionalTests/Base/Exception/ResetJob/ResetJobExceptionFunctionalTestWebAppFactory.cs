using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using UserService.DAL.Repositories;
using UserService.Domain.Entities;
using UserService.Domain.Events;
using UserService.Domain.Interfaces.Repository;
using UserService.Tests.Configurations;

namespace UserService.Tests.FunctionalTests.Base.Exception.ResetJob;

public class ResetJobExceptionFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private static IMock<IBaseRepository<User>> GetExceptionMockUserRepository(
        IBaseRepository<User> originalRepository)
    {
        var mockRepository = new Mock<IBaseRepository<User>>();

        mockRepository.Setup(x => x.GetAll()).Throws(new TestException());
        mockRepository.Setup(x => x.Remove(It.IsAny<User>())).Returns(originalRepository.Remove);
        mockRepository.Setup(x => x.Update(It.IsAny<User>())).Returns(originalRepository.Update);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(originalRepository.CreateAsync);

        return mockRepository;
    }

    private static IMock<IBaseRepository<ProcessedEvent>> GetExceptionMockProcessedEventRepository(
        IBaseRepository<ProcessedEvent> originalRepository)
    {
        var mockRepository = new Mock<IBaseRepository<ProcessedEvent>>();

        mockRepository.Setup(x => x.GetAll()).Throws(new TestException());
        mockRepository.Setup(x => x.Remove(It.IsAny<ProcessedEvent>())).Returns(originalRepository.Remove);
        mockRepository.Setup(x => x.Update(It.IsAny<ProcessedEvent>())).Returns(originalRepository.Update);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<ProcessedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(originalRepository.CreateAsync);

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

            services.RemoveAll<IBaseRepository<ProcessedEvent>>();
            services.AddScoped<IBaseRepository<ProcessedEvent>>(provider =>
            {
                //the dependencies from service provider only apply for this current scope
                //that is why we have to use ActivatorUtilities to transfer dependencies from this scope to callers' scope
                var processedEventRepository =
                    ActivatorUtilities.CreateInstance<BaseRepository<ProcessedEvent>>(provider);
                var exceptionUserRepository = GetExceptionMockProcessedEventRepository(processedEventRepository).Object;

                return exceptionUserRepository;
            });
        });
    }
}