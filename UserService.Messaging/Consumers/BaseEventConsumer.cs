using MassTransit;
using Serilog;
using UserService.Domain.Dtos.User;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;
using UserService.Messaging.Events;

namespace UserService.Messaging.Consumers;

public class BaseEventConsumer(
    IReputationService reputationService,
    ILogger logger) : IConsumer<BaseEvent>
{
    public async Task Consume(ConsumeContext<BaseEvent> context)
    {
        var dto = new ReputationEventDto(context.Message.UserId, context.Message.EntityId,
            Enum.Parse<EntityType>(context.Message.EntityType), Enum.Parse<BaseEventType>(context.Message.EventType));
        var result = await reputationService.ApplyReputationEventAsync(dto, context.CancellationToken);

        LogUpdateResult(context.Message, result);
    }

    private void LogUpdateResult(BaseEvent message, BaseResult result)
    {
        if (!result.IsSuccess)
            logger.Warning(
                "Failed to update reputation. Error: {ErrorMessage}. UserId: {UserId}. Event: {EventType}. EventId: {EventId}. Entity type: {EntityType}. EntityId: {EntityId}",
                result.ErrorMessage, message.UserId, message.EventType, message.EventId, message.EntityType,
                message.EntityId);
        else
            logger.Information(
                "Successfully updated reputation. UserId: {UserId}. Event: {EventType}. EventId: {EventId}. Entity type: {EntityType}. EntityId: {EntityId}",
                message.UserId, message.EventType, message.EventId, message.EntityType, message.EntityId);
    }
}