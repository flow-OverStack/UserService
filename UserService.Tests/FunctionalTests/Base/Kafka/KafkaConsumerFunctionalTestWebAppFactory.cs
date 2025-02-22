using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using UserService.Domain.Settings;
using UserService.ReputationConsumer.Consumers;
using UserService.ReputationConsumer.Events;

namespace UserService.Tests.FunctionalTests.Base.Kafka;

public class KafkaConsumerFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    // Tests will not be implemented via test kafka container
    // because consumer does not work when tests are running.
    // Tests will be implemented via scoped IConsumer<BaseEvent>, which is in DI only in tests
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IOptions<KafkaSettings>>();
            services.Configure<KafkaSettings>(x =>
            {
                x.Host = "test-host";
                x.ReputationTopic = "test-topic";
                x.ReputationConsumerGroup = "test-consumer-group";
            });

            services.AddScoped<IConsumer<BaseEvent>, ReputationEventConsumer>();
        });
    }
}