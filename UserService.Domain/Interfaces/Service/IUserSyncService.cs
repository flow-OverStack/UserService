namespace UserService.Domain.Interfaces.Service;

public interface IUserSyncService
{
    /// <summary>
    ///     Creates the local DB record if it is missing, then updates LastLoginAt.
    /// </summary>
    Task SyncUserOnLoginAsync(string identifier, CancellationToken cancellationToken);
}