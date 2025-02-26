using Xunit;

namespace UserService.Tests.FunctionalTests.Base.Exception.ReputationResetJob;

public class
    ReputationResetJobExceptionBaseFunctionalTest : IClassFixture<
    ReputationResetJobExceptionFunctionalTestWebAppFactory>
{
    protected readonly IServiceProvider ServiceProvider;

    protected ReputationResetJobExceptionBaseFunctionalTest(
        ReputationResetJobExceptionFunctionalTestWebAppFactory factory)
    {
        ServiceProvider = factory.Services;
    }
}