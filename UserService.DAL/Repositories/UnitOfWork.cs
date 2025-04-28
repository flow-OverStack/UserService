using Microsoft.EntityFrameworkCore.Storage;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.DAL.Repositories;

public class UnitOfWork(
    ApplicationDbContext context,
    IBaseRepository<User> users,
    IBaseRepository<UserRole> userRoles)
    : IUnitOfWork
{
    public IBaseRepository<User> Users { get; set; } = users;
    public IBaseRepository<UserRole> UserRoles { get; set; } = userRoles;

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}