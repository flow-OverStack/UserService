using Xunit;

namespace UserService.Tests.FunctionalTests.Base.Kafka;

public class KafkaConsumerBaseFunctionalTest : IClassFixture<KafkaConsumerFunctionalTestWebAppFactory>
{
    protected readonly IServiceProvider ServiceProvider;

    protected KafkaConsumerBaseFunctionalTest(KafkaConsumerFunctionalTestWebAppFactory factory)
    {
        ServiceProvider = factory.Services;
    }
}