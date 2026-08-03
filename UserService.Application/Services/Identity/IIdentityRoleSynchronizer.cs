using UserService.Domain.Entities;

namespace UserService.Application.Services.Identity;

/// <summary>
///     Pushes a user's current role set to the identity server, and schedules a compensating
///     update if the caller's own transaction ends up failing after the sync already landed.
/// </summary>
public interface IIdentityRoleSynchronizer
{
    Task SyncAsync(User user, CancellationToken cancellationToken = default);
    Task SyncAsync(IEnumerable<User> users, CancellationToken cancellationToken = default);
    void ScheduleCompensation(User user);
    void ScheduleCompensation(IEnumerable<User> users);
}
