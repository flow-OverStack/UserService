using AutoMapper;
using UserService.Domain.Dtos.Identity;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Provider;

namespace UserService.Application.Services.Identity;

public class IdentityRoleSynchronizer(
    IMapper mapper,
    IIdentityServer identityServer,
    IIdentityCompensationQueue compensationQueue)
    : IIdentityRoleSynchronizer
{
    public Task SyncAsync(IdentitySyncSourceDto user, CancellationToken cancellationToken = default) =>
        SyncAsync([user], cancellationToken);

    public Task SyncAsync(IEnumerable<IdentitySyncSourceDto> users, CancellationToken cancellationToken = default)
    {
        var updateTasks = users.Select(user =>
        {
            var dto = mapper.Map<IdentityUpdateUserDto>(user);
            return identityServer.UpdateUserAsync(dto, cancellationToken);
        });

        // Identity updates are idempotent (re-sending the same role set is a no-op), so on a
        // partial WhenAll failure it's safe for the caller to compensate every user passed in
        // here, even ones whose update never actually landed.
        return Task.WhenAll(updateTasks);
    }

    public void ScheduleCompensation(IdentitySyncSourceDto user) => ScheduleCompensation([user]);

    public void ScheduleCompensation(IEnumerable<IdentitySyncSourceDto> users)
    {
        foreach (var user in users)
        {
            var dto = mapper.Map<IdentityUpdateUserDto>(user);
            compensationQueue.EnqueueIdentityUserUpdate(dto);
        }
    }
}
