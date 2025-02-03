using Microsoft.EntityFrameworkCore.Storage;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.DAL.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context, IBaseRepository<User> users,
        IBaseRepository<UserRole> userRoles)
    {
        _context = context;
        Users = users;
        UserRoles = userRoles;
    }


    public IBaseRepository<User> Users { get; set; }
    public IBaseRepository<UserRole> UserRoles { get; set; }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}