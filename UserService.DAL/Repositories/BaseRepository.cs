using EFCore.BulkExtensions;
using UserService.Domain.Interfaces.Repository;

namespace UserService.DAL.Repositories;

public class BaseRepository<TEntity>(ApplicationDbContext dbContext) : IBaseRepository<TEntity>
    where TEntity : class
{
    public IQueryable<TEntity> GetAll()
    {
        return dbContext.Set<TEntity>();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await dbContext.AddAsync(entity, cancellationToken);

        return entity;
    }

    public TEntity Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        dbContext.Update(entity);

        return entity;
    }

    public TEntity Remove(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        dbContext.Remove(entity);

        return entity;
    }

    public async Task BulkUpdateAsync(IEnumerable<TEntity> entities, IEnumerable<string> propertiesToUpdate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        await dbContext.BulkUpdateAsync(entities,
            config => config.PropertiesToIncludeOnUpdate = propertiesToUpdate.ToList(),
            cancellationToken: cancellationToken);
    }
}