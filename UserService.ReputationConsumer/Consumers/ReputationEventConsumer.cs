using MassTransit;
using Serilog;
using UserService.Domain.Dto.User;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Result;
using UserService.ReputationConsumer.Events;
using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Consumers;

public class ReputationEventConsumer(
    IReputationStrategyResolver reputationResolver,
    IReputationService reputationService,
    ILogger logger) : IConsumer<BaseEvent>
{
    public async Task Consume(ConsumeContext<BaseEvent> context)
    {
        try
        {
            var strategy = reputationResolver.Resolve(context.Message.EventType);
            var reputationChange = strategy.CalculateReputationChange();

            var result = await UpdateReputationAsync(context.Message, reputationChange);
            LogReputationResult(context.Message, result);
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to update reputation: {ErrorMessage} UserId: {UserId}.Event: {EventType}.",
                e.Message, context.Message.UserId, context.Message.EventType);
        }
    }

    private async Task<BaseResult<ReputationDto>> UpdateReputationAsync(BaseEvent message, int reputationChange)
    {
        BaseResult<ReputationDto> result;
        if (reputationChange > 0)
            result = await reputationService.IncreaseReputationAsync(
                new ReputationIncreaseDto(message.UserId, reputationChange));
        else
            result = await reputationService.DecreaseReputationAsync(
                new ReputationDecreaseDto(message.UserId, -reputationChange));

        return result;
    }

    private void LogReputationResult(BaseEvent message, BaseResult<ReputationDto> result)
    {
        if (!result.IsSuccess)
            logger.Warning(
                "Failed to update reputation. Error: {ErrorMessage}. UserId: {UserId}. Event: {EventType}.",
                result.ErrorMessage, message.UserId, message.EventType);
        else
            logger.Information("Successfully updated reputation. UserId: {UserId}. Event: {EventType}.",
                message.UserId, message.EventType);
    }
}