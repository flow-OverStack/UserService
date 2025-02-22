using Microsoft.Extensions.DependencyInjection;
using UserService.Tests.FunctionalTests.Configurations;
using Xunit;

namespace UserService.Tests.FunctionalTests.Base.Kafka;

public class KafkaConsumerBaseFunctionalTest : IClassFixture<KafkaConsumerFunctionalTestWebAppFactory>, IAsyncLifetime
{
    protected readonly IServiceProvider ServiceProvider;

    protected KafkaConsumerBaseFunctionalTest(KafkaConsumerFunctionalTestWebAppFactory factory)
    {
        ServiceProvider = factory.Services;
    }

    public Task InitializeAsync()
    {
        using var scope = ServiceProvider.CreateScope();
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