using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UserService.Tests.FunctionalTests.Configurations.TestServices;

namespace UserService.Tests.FunctionalTests.Base.Exception.GraphQl;

public class NullHttpContextMeGraphQlTestWebAppFactory : FunctionalTestWebAppFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IHttpContextAccessor>();
            services.AddSingleton<IHttpContextAccessor, NullHttpContextAccessor>();
        });
    }
}