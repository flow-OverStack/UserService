using MassTransit;
using UserService.ReputationConsumer.Events;

namespace UserService.ReputationConsumer.Consumers;

public class ReputationEventConsumer : IConsumer<BaseEvent>
{
    public Task Consume(ConsumeContext<BaseEvent> context)
    {
        throw new NotImplementedException();
    }
}