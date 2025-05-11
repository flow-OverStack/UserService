using Xunit;

namespace UserService.Tests.FunctionalTests.Base.Exception.ResetJob;

public class
    ExceptionResetJobFunctionalTest : IClassFixture<
    ExceptionResetJobFunctionalTestWebAppFactory>
{
    protected readonly IServiceProvider ServiceProvider;

    protected ExceptionResetJobFunctionalTest(
        ExceptionResetJobFunctionalTestWebAppFactory factory)
    {
        ServiceProvider = factory.Services;
    }
}