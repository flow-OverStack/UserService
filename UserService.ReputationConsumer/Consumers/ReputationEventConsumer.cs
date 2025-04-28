using MassTransit;
using Serilog;
using UserService.Domain.Dto.User;
using UserService.Domain.Events;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Result;
using UserService.ReputationConsumer.Interfaces;
using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Consumers;

public class ReputationEventConsumer(
    IReputationStrategyResolver reputationResolver,
    IReputationService reputationService,
    IProcessedEventRepository processedEventRepository,
    ILogger logger) : IConsumer<BaseEvent>
{
    public async Task Consume(ConsumeContext<BaseEvent> context)
    {
        try
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

            var result = await UpdateReputationAsync(context.Message, reputationChange);

            await processedEventRepository.MarkAsProcessedAsync(context.Message.EventId);

            LogReputationResult(context.Message, result);
        }
        catch (Exception e)
        {
            logger.Error(e,
                "Failed to update reputation: {ErrorMessage} UserId: {UserId}. Event: {EventType}. EventId: {EventId}",
                e.Message, context.Message.UserId, context.Message.EventType, context.Message.EventId);
        }
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

    private void LogReputationResult(BaseEvent message, BaseResult<ReputationDto> result)
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