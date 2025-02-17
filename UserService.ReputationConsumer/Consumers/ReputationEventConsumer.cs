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

            BaseResult<ReputationDto> result;
            if (reputationChange > 0)
                result = await reputationService.IncreaseReputationAsync(
                    new ReputationIncreaseDto(context.Message.UserId, reputationChange));
            else
                result = await reputationService.DecreaseReputationAsync(
                    new ReputationDecreaseDto(context.Message.UserId, -reputationChange));

            if (!result.IsSuccess)
                logger.Warning(
                    "Failed to update reputation. Error: {ErrorMessage}. UserId: {UserId}. Event: {EventType}.",
                    result.ErrorMessage, context.Message.UserId, context.Message.EventType);
            else
                logger.Information("Successfully updated reputation. UserId: {UserId}. Event: {EventType}.",
                    context.Message.UserId, context.Message.EventType);
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to update reputation: {ErrorMessage} UserId: {UserId}.Event: {EventType}.",
                e.Message, context.Message.UserId, context.Message.EventType);
        }
    }
}