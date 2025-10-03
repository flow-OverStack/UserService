using UserService.Domain.Dtos.User;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

public interface IUserActivityDatabaseService
{
    /// <summary>
    ///     Synchronizes the heartbeats from cache to database
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<BaseResult<SyncedHeartbeatsDto>> SyncHeartbeatsToDatabaseAsync(
        CancellationToken cancellationToken = default);
}