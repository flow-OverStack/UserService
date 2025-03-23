using Xunit;

namespace UserService.Tests.FunctionalTests.Base.Exception.ResetJob;

public class
    ResetJobExceptionBaseFunctionalTest : IClassFixture<
    ResetJobExceptionFunctionalTestWebAppFactory>
{
    protected readonly IServiceProvider ServiceProvider;

    protected ResetJobExceptionBaseFunctionalTest(
        ResetJobExceptionFunctionalTestWebAppFactory factory)
    {
        ServiceProvider = factory.Services;
    }
}