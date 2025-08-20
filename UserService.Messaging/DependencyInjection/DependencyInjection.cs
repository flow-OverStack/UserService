using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UserService.Domain.Events;
using UserService.Messaging.Consumers;
using UserService.Messaging.Filters;
using UserService.Messaging.Interfaces;
using UserService.Messaging.Messages;
using UserService.Messaging.Processors;
using UserService.Messaging.Settings;
using UserService.Messaging.Strategies.Reputation;
using UserService.Messaging.Strategies.Reputation.Base;
using UserService.Messaging.Strategies.Reputation.Strategies;

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
        services.InitStrategies();
        services.InitEventProcessors();
    }

    private static void InitMassTransit(this IServiceCollection services)
    {
        services.AddMassTransit(configurator =>
        {
            configurator.SetKebabCaseEndpointNameFormatter(); // just in case

            configurator.UsingInMemory();

            configurator.AddRider(rider =>
            {
                rider.AddConsumer<ReputationEventConsumer>();

                // Scope is not created because IOptions<KafkaSettings> is a singleton
                using var provider = rider.BuildServiceProvider();
                var kafkaSettings = provider.GetRequiredService<IOptions<KafkaSettings>>().Value;

                rider.AddProducer<BaseEvent>(kafkaSettings.ReputationTopic,
                    new ProducerConfig { Acks = Acks.All, EnableIdempotence = false });
                rider.AddProducer<FaultedMessage>(kafkaSettings.DeadLetterTopic,
                    new ProducerConfig { Acks = Acks.All, EnableIdempotence = false });

                rider.UsingKafka((context, factoryConfigurator) =>
                {
                    var kafkaSettings = context.GetRequiredService<IOptions<KafkaSettings>>().Value;

                    factoryConfigurator.Host(kafkaSettings.Host);

                    factoryConfigurator.TopicEndpoint<BaseEvent>(kafkaSettings.ReputationTopic,
                        kafkaSettings.ReputationConsumerGroup,
                        cfg =>
                        {
                            cfg.ConfigureConsumer<ReputationEventConsumer>(context);

                            cfg.UseFilter(
                                new DeadLetterFilter<BaseEvent>(context.GetRequiredService<IServiceScopeFactory>()));
                            cfg.UseFilter(new RetryAndRedeliveryFilter<BaseEvent>());
                        }
                    );
                });
            });
        });
    }

    private static void InitStrategies(this IServiceCollection services)
    {
        services.AddTransient<IReputationStrategy, AnswerAcceptedStrategy>();
        services.AddTransient<IReputationStrategy, AnswerDownvoteStrategy>();
        services.AddTransient<IReputationStrategy, AnswerUpvoteStrategy>();
        services.AddTransient<IReputationStrategy, DownvoteGivenForAnswerStrategy>();
        services.AddTransient<IReputationStrategy, QuestionDownvoteStrategy>();
        services.AddTransient<IReputationStrategy, QuestionUpvoteStrategy>();
        services.AddTransient<IReputationStrategy, UserAcceptedAnswerStrategy>();
        services.AddSingleton<IReputationStrategyResolver, ReputationStrategyResolver>();
    }

    private static void InitEventProcessors(this IServiceCollection services)
    {
        services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();
    }
}