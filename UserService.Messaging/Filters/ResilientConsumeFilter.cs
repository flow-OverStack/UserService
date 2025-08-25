using Hangfire;
using MassTransit;
using Newtonsoft.Json;
using UserService.Messaging.Messages;

namespace UserService.Messaging.Filters;

public class ResilientConsumeFilter<TEvent> : IFilter<ConsumeContext<TEvent>> where TEvent : class
{
    private const string RedeliveryCountHeader = "RedeliveryCount";

    private static TimeSpan[] ScheduledRedeliveryIntervals =>
    [
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(10),
        TimeSpan.FromHours(1),
        TimeSpan.FromHours(12),
        TimeSpan.FromHours(24)
    ];

    private static TimeSpan[] ImmediateRetryIntervals =>
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
                        await Task.Delay(retry);
                        await next.Send(context);
                        return;
                    }
                    catch (Exception)
                    {
                        // Just waiting for the next retry
                    }


            var redeliveryCount = 0;
            if (redeliveryCountObj is string s && int.TryParse(s, out var parsed))
                redeliveryCount = ++parsed;

            if (redeliveryCount >= ScheduledRedeliveryIntervals.Length)
            {
                MoveToDeadLetterQueue(context, e);
                throw;
            }

            var message = context.Message;
            BackgroundJob.Schedule<RedeliveryJob>(
                job => job.PublishWithRedelivery(message, redeliveryCount),
                ScheduledRedeliveryIntervals[redeliveryCount]);

            // Throwing the exception for killswitch to handle it
            throw;
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope(nameof(ResilientConsumeFilter<TEvent>));
        context.Add("maxRetries", ScheduledRedeliveryIntervals.Length);
    }

    private static void MoveToDeadLetterQueue(ConsumeContext<TEvent> context, Exception e)
    {
        var message = new FaultedMessage
        {
            SerializedMessage = JsonConvert.SerializeObject(context.Message),
            ErrorMessage = e.Message,
            StackTrace = e.StackTrace!,
            Source = context.DestinationAddress!.AbsoluteUri
        };
        var ct = context.CancellationToken;

        BackgroundJob.Enqueue<ITopicProducer<FaultedMessage>>(producer =>
            producer.Produce(message, ct));
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