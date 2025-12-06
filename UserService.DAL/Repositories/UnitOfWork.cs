using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Database;
using UserService.Domain.Interfaces.Repository;

namespace UserService.DAL.Repositories;

public class UnitOfWork(
    ApplicationDbContext context,
    IBaseRepository<User> users,
    IBaseRepository<Role> roles,
    IBaseRepository<ReputationRule> reputationRules,
    IBaseRepository<ReputationRecord> reputationRecords)
    : IUnitOfWork
{
    public IBaseRepository<User> Users { get; set; } = users;
    public IBaseRepository<Role> Roles { get; set; } = roles;
    public IBaseRepository<ReputationRule> ReputationRules { get; set; } = reputationRules;
    public IBaseRepository<ReputationRecord> ReputationRecords { get; set; } = reputationRecords;

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        return new DbContextTransaction(transaction);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}