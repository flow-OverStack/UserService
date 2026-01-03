using Microsoft.EntityFrameworkCore.Storage;
using UserService.Domain.Interfaces.Database;

namespace UserService.DAL.Transactions;

public class OwnedDbContextTransaction(IDbContextTransaction transaction) : ITransaction
{
    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return transaction.CommitAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        return transaction.RollbackAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await transaction.DisposeAsync();
        GC.SuppressFinalize(this); // In case of derived classes with finalizers
    }
}