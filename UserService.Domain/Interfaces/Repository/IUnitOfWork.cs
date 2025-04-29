using Microsoft.EntityFrameworkCore.Storage;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Database;

namespace UserService.Domain.Interfaces.Repository;

public interface IUnitOfWork : IStateSaveChanges
{
    IBaseRepository<User> Users { get; set; }
    IBaseRepository<Role> Roles { get; set; }

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}