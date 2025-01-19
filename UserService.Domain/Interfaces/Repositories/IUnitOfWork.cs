using Microsoft.EntityFrameworkCore.Storage;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Databases;

namespace UserService.Domain.Interfaces.Repositories;

public interface IUnitOfWork : IStateSaveChanges
{
    IBaseRepository<User> Users { get; set; }

    IBaseRepository<UserRole> UserRoles { get; set; }

    Task<IDbContextTransaction> BeginTransactionAsync();
}