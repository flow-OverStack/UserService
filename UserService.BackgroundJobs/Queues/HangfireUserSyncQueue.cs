using Hangfire;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Service;

namespace UserService.BackgroundJobs.Queues;

public class HangfireUserSyncQueue(IBackgroundJobClient client) : IUserSyncQueue
{
    public void EnqueueLoginSync(string identifier) =>
        client.Enqueue<IUserSyncService>(svc => svc.SyncUserOnLoginAsync(identifier, CancellationToken.None));
}
