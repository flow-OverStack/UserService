using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UserService.Domain.Settings;
using UserService.ReputationConsumer.Consumers;
using UserService.ReputationConsumer.Events;
using UserService.ReputationConsumer.Strategy.Reputation;
using UserService.ReputationConsumer.Strategy.Reputation.Base;
using UserService.ReputationConsumer.Strategy.Reputation.Strategies;

namespace UserService.ReputationConsumer.DependencyInjection;

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

                rider.AddProducer<BaseEvent>("main-events-topic"); //TODO remove this line

                rider.UsingKafka((context, factoryConfigurator) =>
                {
                    var kafkaSettings = context.GetRequiredService<IOptions<KafkaSettings>>().Value;

                    factoryConfigurator.Host(kafkaSettings.Host);

                    factoryConfigurator.TopicEndpoint<BaseEvent>(kafkaSettings.ReputationTopic,
                        kafkaSettings.ReputationConsumerGroup,
                        e => { e.ConfigureConsumer<ReputationEventConsumer>(context); });
                });
            });
        });
    }

    private static void InitStrategies(this IServiceCollection services)
    {
        services.AddTransient<IReputationStrategy, AnswerAcceptedStrategy>();
        services.AddTransient<IReputationStrategy, AnswerDownvoteStrategy>();
        services.AddTransient<IReputationStrategy, AnswerUpvoteStrategy>();
        services.AddTransient<IReputationStrategy, DownvoteGivenStrategy>();
        services.AddTransient<IReputationStrategy, QuestionDownvoteStrategy>();
        services.AddTransient<IReputationStrategy, QuestionUpvoteStrategy>();
        services.AddTransient<IReputationStrategy, UserAcceptedAnswerStrategy>();
        services.AddSingleton<IReputationStrategyResolver, ReputationStrategyResolver>();
    }
}