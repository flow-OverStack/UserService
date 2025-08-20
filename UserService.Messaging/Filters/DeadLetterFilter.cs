using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using UserService.Messaging.Messages;

namespace UserService.Messaging.Filters;

public class DeadLetterFilter<TEvent>(IServiceScopeFactory scopeFactory)
    : IFilter<ConsumeContext<TEvent>> where TEvent : class
{
    public async Task Send(ConsumeContext<TEvent> context, IPipe<ConsumeContext<TEvent>> next)
    {
        try
        {
            await next.Send(context);
        }
        catch (Exception e)
        {
            using var scope = scopeFactory.CreateScope();
            var producer = scope.ServiceProvider.GetRequiredService<ITopicProducer<FaultedMessage>>();

            var message = new FaultedMessage
            {
                SerializedMessage = JsonConvert.SerializeObject(context.Message),
                ErrorMessage = e.Message,
                StackTrace = e.StackTrace!,
                Source = context.DestinationAddress!.AbsoluteUri
            };

            await producer.Produce(message, context.CancellationToken);
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope(nameof(RetryAndRedeliveryFilter<TEvent>));
    }
}