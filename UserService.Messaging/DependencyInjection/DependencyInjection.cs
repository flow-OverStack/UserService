using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UserService.Messaging.Consumers;
using UserService.Messaging.Events;
using UserService.Messaging.Filters;
using UserService.Messaging.Interfaces;
using UserService.Messaging.Messages;
using UserService.Messaging.Services;
using UserService.Messaging.Settings;

namespace UserService.Messaging.DependencyInjection;

public static class DependencyInjection
{
    /// <summary>
    ///     Adds message brokers with MassTransit
    /// </summary>
    /// <param name="services"></param>
    public static void AddMassTransitServices(this IServiceCollection services)
    {
        services.InitMassTransit();
        services.InitEventServices();
    }

    private static void InitMassTransit(this IServiceCollection services)
    {
        services.AddMassTransit(configurator =>
        {
            configurator.SetKebabCaseEndpointNameFormatter(); // just in case

            configurator.UsingInMemory();

            configurator.AddRider(rider =>
            {
                rider.AddConsumer<BaseEventConsumer>();

                // Scope is not created because IOptions<KafkaSettings> is a singleton
                using var provider = rider.BuildServiceProvider();
                var kafkaSettings = provider.GetRequiredService<IOptions<KafkaSettings>>().Value;

                var defaultProducerConfig = new ProducerConfig { Acks = Acks.All, EnableIdempotence = false };

                rider.AddProducer<BaseEvent>(kafkaSettings.BaseEventTopic, defaultProducerConfig);
                rider.AddProducer<FaultedMessage>(kafkaSettings.DeadLetterTopic, defaultProducerConfig);

                rider.UsingKafka((context, factoryConfigurator) =>
                {
                    var kafkaSettings = context.GetRequiredService<IOptions<KafkaSettings>>().Value;

                    factoryConfigurator.Host(kafkaSettings.Host);

                    factoryConfigurator.TopicEndpoint<BaseEvent>(kafkaSettings.BaseEventTopic,
                        kafkaSettings.BaseEventConsumerGroup,
                        cfg =>
                        {
                            cfg.ConfigureConsumer<BaseEventConsumer>(context);
                            cfg.ConfigureKafkaEndpointDefaults(context);
                        }
                    );
                });
            });
        });
    }

    private static void InitEventServices(this IServiceCollection services)
    {
        services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();
        services.AddScoped<IProcessedEventsResetService, ProcessedEventsResetService>();
    }

    private static void ConfigureKafkaEndpointDefaults<TEvent>(
        this IKafkaTopicReceiveEndpointConfigurator<Ignore, TEvent> cfg,
        IRiderRegistrationContext context) where TEvent : class
    {
        cfg.CreateIfMissing();

        cfg.UseConsumeFilter(typeof(ResilientConsumeFilter<>), context);
        cfg.UseConsumeFilter(typeof(ProcessedEventFilter<>), context);

        cfg.UseKillSwitch(options => options
            .SetActivationThreshold(10)
            .SetTrackingPeriod(TimeSpan.FromMinutes(3))
            .SetTripThreshold(15)
            .SetRestartTimeout(TimeSpan.FromMinutes(1)));
    }
}