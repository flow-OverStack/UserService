using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.DAL.Interceptors;
using ILogger = Serilog.ILogger;

namespace UserService.DAL;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger logger)
    : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(logger.Information, LogLevel.Information);
        optionsBuilder.AddInterceptors(new DateInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}