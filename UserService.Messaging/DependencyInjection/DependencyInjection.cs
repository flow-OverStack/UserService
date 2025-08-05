using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UserService.Domain.Events;
using UserService.Messaging.Consumers;
using UserService.Messaging.Interfaces;
using UserService.Messaging.Processors;
using UserService.Messaging.Settings;
using UserService.Messaging.Strategies.Reputation;
using UserService.Messaging.Strategies.Reputation.Base;
using UserService.Messaging.Strategies.Reputation.Strategies;

namespace UserService.Messaging.DependencyInjection;

public static class DependencyInjection
{
    private static TimeSpan[] ImmediateRetryIntervals =>
    [
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(15),
        TimeSpan.FromSeconds(30)
    ];

    /*private static TimeSpan[] ScheduledRedeliveryIntervals =>
    [
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(10),
        TimeSpan.FromHours(1),
        TimeSpan.FromHours(12),
        TimeSpan.FromHours(24)
    ];*/

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

                rider.UsingKafka((context, factoryConfigurator) =>
                {
                    var kafkaSettings = context.GetRequiredService<IOptions<KafkaSettings>>().Value;

                    factoryConfigurator.Host(kafkaSettings.Host);

                    factoryConfigurator.TopicEndpoint<BaseEvent>(kafkaSettings.ReputationTopic,
                        kafkaSettings.ReputationConsumerGroup,
                        cfg =>
                        {
                            cfg.ConfigureConsumer<ReputationEventConsumer>(context);

                            // Kafka-only configuration
                            cfg.UseMessageRetry(r => r.Intervals(ImmediateRetryIntervals));

                            cfg.UseInMemoryOutbox(context);
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