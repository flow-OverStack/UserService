using UserService.Domain.Dtos.User;

namespace UserService.Domain.Interfaces.Repository.Cache;

public interface IUserActivityCacheRepository
{
    /// Retrieves a collection of valid user activities from the cache.
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains a collection of UserActivityDto
    ///     representing user activities.
    /// </returns>
    Task<IEnumerable<UserActivityDto>> GetValidActivitiesAsync(CancellationToken cancellationToken = default);

    /// Deletes all user activities from the cache.
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    Task DeleteAllActivitiesAsync(CancellationToken cancellationToken = default);

    /// Adds a user activity to the cache.
    /// <param name="dto">
    ///     An object containing the user activity details, such as the user ID and the last login timestamp.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    Task AddActivityAsync(UserActivityDto dto, CancellationToken cancellationToken = default);
}