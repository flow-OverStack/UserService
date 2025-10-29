using MassTransit;
using Serilog;
using UserService.Domain.Dtos.User;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;
using UserService.Messaging.Events;
using UserService.Messaging.Interfaces;
using UserService.Messaging.Strategies.Reputation.Base;

namespace UserService.Messaging.Consumers;

public class ReputationEventConsumer(
    IReputationStrategyResolver reputationResolver,
    IReputationService reputationService,
    IProcessedEventRepository processedEventRepository,
    ILogger logger) : IConsumer<BaseEvent>
{
    public async Task Consume(ConsumeContext<BaseEvent> context)
    {
        if (await processedEventRepository.IsEventProcessedAsync(context.Message.EventId))
        {
            logger.Warning(
                "Event has already been processed: UserId: {UserId}. Event: {EventType}. EventId: {EventId}",
                context.Message.UserId, context.Message.EventType, context.Message.EventId);
            return;
        }

        var strategy = reputationResolver.Resolve(context.Message.EventType);
        var reputationChange = strategy.CalculateReputationChange();

        if (context.Message.CancelsEvent != null)
        {
            var cancelStrategy = reputationResolver.Resolve(context.Message.CancelsEvent);
            var cancelReputationChange = -cancelStrategy.CalculateReputationChange();

            reputationChange += cancelReputationChange;
        }

        var result = await UpdateReputationAsync(context.Message, reputationChange);

        await processedEventRepository.MarkAsProcessedAsync(context.Message.EventId);

        LogUpdateResult(context.Message, result);
    }

    private async Task<BaseResult<ReputationDto>> UpdateReputationAsync(BaseEvent message, int reputationChange,
        CancellationToken cancellationToken = default)
    {
        BaseResult<ReputationDto> result;
        if (reputationChange > 0)
            result = await reputationService.IncreaseReputationAsync(
                new ReputationIncreaseDto(message.UserId, reputationChange), cancellationToken);
        else
            result = await reputationService.DecreaseReputationAsync(
                new ReputationDecreaseDto(message.UserId, -reputationChange), cancellationToken);

        return result;
    }

    private void LogUpdateResult(BaseEvent message, BaseResult<ReputationDto> result)
    {
        if (!result.IsSuccess)
            logger.Warning(
                "Failed to update reputation. Error: {ErrorMessage}. UserId: {UserId}. Event: {EventType}. EventId: {EventId}",
                result.ErrorMessage, message.UserId, message.EventType, message.EventId);
        else
            logger.Information(
                "Successfully updated reputation. UserId: {UserId}. Event: {EventType}. EventId: {EventId}",
                message.UserId, message.EventType, message.EventId);
    }
}