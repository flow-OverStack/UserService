using Hangfire;
using UserService.Domain.Dtos.Identity;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Provider;

namespace UserService.BackgroundJobs.Queues;

public class HangfireIdentityCompensationQueue(IBackgroundJobClient client) : IIdentityCompensationQueue
{
    public void EnqueueIdentityUserUpdate(IdentityUpdateUserDto dto) =>
        client.Enqueue<IIdentityServer>(server => server.UpdateUserAsync(dto, CancellationToken.None));

    public void EnqueueIdentityUserDeletion(string identityId) =>
        client.Enqueue<IIdentityServer>(server => server.DeleteUserAsync(new IdentityUserIdDto(identityId)));
}
