namespace UserService.Domain.Interfaces.Provider;

public interface IUserSyncQueue
{
    void EnqueueLoginSync(string identifier);
}
