using Microsoft.Extensions.DependencyInjection;
using UserService.Tests.FunctionalTests.Configurations;

namespace UserService.Tests.FunctionalTests;

public class SequentialFunctionalTest : BaseFunctionalTest
{
    protected SequentialFunctionalTest(FunctionalTestWebAppFactory factory) : base(factory)
    {
        ServiceScope = factory.Services.CreateScope();
    }

    private static IServiceScope ServiceScope { get; set; }

    protected static void ResetDb()
    {
        ServiceScope.PrepPopulation();
    }
}