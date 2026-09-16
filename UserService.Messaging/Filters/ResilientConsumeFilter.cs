using Hangfire;
using MassTransit;
using Newtonsoft.Json;
using Serilog;
using UserService.Messaging.Messages;

namespace UserService.Messaging.Filters;

public class ResilientConsumeFilter<TEvent>(IBackgroundJobClient backgroundJob, ILogger logger)
    : IFilter<ConsumeContext<TEvent>> where TEvent : class
{
    private const string RedeliveryCountHeader = "RedeliveryCount";

    private static readonly TimeSpan[] ScheduledRedeliveryIntervals =
    [
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(10),
        TimeSpan.FromHours(1),
        TimeSpan.FromHours(12),
        TimeSpan.FromHours(24)
    ];

    private static readonly TimeSpan[] ImmediateRetryIntervals =
    [
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(15),
        TimeSpan.FromSeconds(30)
    ];

    public async Task Send(ConsumeContext<TEvent> context, IPipe<ConsumeContext<TEvent>> next)
    {
        try
        {
            await next.Send(context);
        }
        catch (Exception e)
        {
            var redeliveryHeaderExists =
                context.Headers.TryGetHeader(RedeliveryCountHeader, out var redeliveryCountObj);

            if (!redeliveryHeaderExists)
                foreach (var retry in ImmediateRetryIntervals)
                    try
                    {
                        await Task.Delay(retry, context.CancellationToken);
                        await next.Send(context);
                        return;
                    }
                    catch (Exception retryException)
                    {
                        logger.Warning(retryException,
                            "Immediate retry failed for message {MessageId}, retrying in {Retry}",
                            context.MessageId, retry);
                    }


            var redeliveryCount = 0;
            if (redeliveryCountObj is string s && int.TryParse(s, out var parsed))
                redeliveryCount = parsed + 1;

            if (redeliveryCount >= ScheduledRedeliveryIntervals.Length)
            {
                MoveToDeadLetterQueue(context, e);
                throw;
            }

            var message = context.Message;
            backgroundJob.Schedule<RedeliveryJob>(
                job => job.PublishWithRedelivery(message, redeliveryCount),
                ScheduledRedeliveryIntervals[redeliveryCount]);

            // Throwing the exception for killswitch to handle it
            throw;
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope(nameof(ResilientConsumeFilter<>));
        context.Add("maxRetries", ScheduledRedeliveryIntervals.Length);
    }

    private void MoveToDeadLetterQueue(ConsumeContext<TEvent> context, Exception e)
    {
        var message = new FaultedMessage
        {
            SerializedMessage = JsonConvert.SerializeObject(context.Message),
            ErrorMessage = e.Message,
            StackTrace = e.StackTrace!,
            Source = context.DestinationAddress!.AbsoluteUri
        };

        backgroundJob.Enqueue<ITopicProducer<FaultedMessage>>(producer =>
            producer.Produce(message, CancellationToken.None));
    }

    private sealed class RedeliveryJob(ITopicProducer<TEvent> producer)
    {
        public Task PublishWithRedelivery(TEvent message, int redeliveryCount)
        {
            return producer.Produce(message,
                Pipe.Execute<KafkaSendContext<TEvent>>(ctx =>
                    ctx.Headers.Set(RedeliveryCountHeader, redeliveryCount)));
        }
    }
}
