using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

public interface IUserActivityService
{
    /// <summary>
    ///     Registers user's last activity time
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<BaseResult> RegisterHeartbeatAsync(long id, CancellationToken cancellationToken = default);
}