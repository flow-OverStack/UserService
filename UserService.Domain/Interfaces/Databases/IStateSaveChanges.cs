namespace UserService.Domain.Interfaces.Databases;

public interface IStateSaveChanges
{
    Task<int> SaveChangesAsync();
}