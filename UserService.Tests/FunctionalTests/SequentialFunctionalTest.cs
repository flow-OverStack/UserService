using Microsoft.Extensions.DependencyInjection;
using UserService.Tests.FunctionalTests.Configurations;
using Xunit;

namespace UserService.Tests.FunctionalTests;

public class SequentialFunctionalTest : BaseFunctionalTest, IAsyncLifetime
{
    private readonly IServiceProvider _servicesProvider;

    protected SequentialFunctionalTest(FunctionalTestWebAppFactory factory) : base(factory)
    {
        _servicesProvider = factory.Services;
    }

    public Task InitializeAsync()
    {
        using var scope = _servicesProvider.CreateScope();
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