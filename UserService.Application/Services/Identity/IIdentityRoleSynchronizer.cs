using UserService.Domain.Dtos.Identity;

namespace UserService.Application.Services.Identity;

/// <summary>
///     Pushes a user's current role set to the identity server, and schedules a compensating
///     update if the caller's own transaction ends up failing after the sync already landed.
/// </summary>
public interface IIdentityRoleSynchronizer
{
    Task SyncAsync(IdentitySyncSourceDto user, CancellationToken cancellationToken = default);
    Task SyncAsync(IEnumerable<IdentitySyncSourceDto> users, CancellationToken cancellationToken = default);
    void ScheduleCompensation(IdentitySyncSourceDto user);
    void ScheduleCompensation(IEnumerable<IdentitySyncSourceDto> users);
}
