using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Repositories;
using UserService.Tests.Configurations;

namespace UserService.Tests.FunctionalTests.Base.Exception.GraphQl;

public class GraphQlExceptionFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private static Mock<IBaseRepository<User>> GetExceptionMockUserRepository()
    {
        var mockRepository = new Mock<IBaseRepository<User>>();

        mockRepository.Setup(x => x.GetAll()).Throws(new TestException());

        return mockRepository;
    }

    private static Mock<IBaseRepository<Role>> GetExceptionMockRoleRepository()
    {
        var mockRepository = new Mock<IBaseRepository<Role>>();

        mockRepository.Setup(x => x.GetAll()).Throws(new TestException());

        return mockRepository;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IBaseRepository<User>>();
            services.AddScoped<IBaseRepository<User>>(_ =>
            {
                var userRepository = GetExceptionMockUserRepository().Object;
                return userRepository;
            });

            services.RemoveAll<IBaseRepository<Role>>();
            services.AddScoped<IBaseRepository<Role>>(_ =>
            {
                var roleRepository = GetExceptionMockRoleRepository().Object;
                return roleRepository;
            });
        });
    }
}