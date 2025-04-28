using Microsoft.EntityFrameworkCore.Storage;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Databases;

namespace UserService.Domain.Interfaces.Repositories;

public interface IUnitOfWork : IStateSaveChanges
{
    IBaseRepository<User> Users { get; set; }

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}