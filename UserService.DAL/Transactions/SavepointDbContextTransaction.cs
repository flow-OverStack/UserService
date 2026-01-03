using Microsoft.EntityFrameworkCore.Storage;
using UserService.Domain.Interfaces.Database;

namespace UserService.DAL.Transactions;

public class SavepointDbContextTransaction(IDbContextTransaction transaction, string currentTransactionName)
    : ITransaction
{
    private bool _completed;

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await transaction.ReleaseSavepointAsync(currentTransactionName, cancellationToken);
        _completed = true;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await transaction.RollbackToSavepointAsync(currentTransactionName, cancellationToken);
        _completed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_completed)
            try
            {
                await RollbackAsync();
            }
            catch (Exception)
            {
                // Ignore exceptions during rollback in Dispose
            }

        GC.SuppressFinalize(this); // In case of derived classes with finalizers
    }
}