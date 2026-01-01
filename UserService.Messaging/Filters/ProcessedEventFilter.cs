using MassTransit;
using Serilog;
using UserService.Messaging.Events;
using UserService.Messaging.Interfaces;

namespace UserService.Messaging.Filters;

public class ProcessedEventFilter<TEvent>(IProcessedEventRepository processedEventRepository, ILogger logger)
    : IFilter<ConsumeContext<TEvent>> where TEvent : BaseEvent
{
    public async Task Send(ConsumeContext<TEvent> context, IPipe<ConsumeContext<TEvent>> next)
    {
        if (await processedEventRepository.IsEventProcessedAsync(context.Message.EventId))
        {
            logger.Warning(
                "Event has already been processed: UserId: {UserId}. Event: {EventType}. EventId: {EventId}. Entity type: {EntityType}. EntityId: {EntityId}",
                context.Message.UserId, context.Message.EventType, context.Message.EventId, context.Message.EntityType,
                context.Message.EntityId);
            return;
        }

        await next.Send(context);

        await processedEventRepository.MarkAsProcessedAsync(context.Message.EventId);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope(nameof(ProcessedEventFilter<>));
    }
}