using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Database;

namespace UserService.Domain.Interfaces.Repository;

public interface IUnitOfWork : IStateSaveChanges
{
    IBaseRepository<User> Users { get; set; }
    IBaseRepository<Role> Roles { get; set; }
    IBaseRepository<ReputationRule> ReputationRules { get; set; }
    IBaseRepository<ReputationRecord> ReputationRecords { get; set; }

    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}