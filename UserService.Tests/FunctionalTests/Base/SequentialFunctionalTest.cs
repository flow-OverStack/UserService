using Microsoft.Extensions.DependencyInjection;
using UserService.Tests.FunctionalTests.Configurations;
using Xunit;

namespace UserService.Tests.FunctionalTests.Base;

public class SequentialFunctionalTest : BaseFunctionalTest, IAsyncLifetime
{
    public readonly IServiceProvider ServicesProvider;

    protected SequentialFunctionalTest(FunctionalTestWebAppFactory factory) : base(factory)
    {
        ServicesProvider = factory.Services;
    }

    public Task InitializeAsync()
    {
        using var scope = ServicesProvider.CreateScope();
        ResetDb(scope);
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private static void ResetDb(IServiceScope scope)
    {
        scope.PrepPopulation();
    }
}