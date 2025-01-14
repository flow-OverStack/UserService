using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.DAL.Interceptors;
using ILogger = Serilog.ILogger;

namespace UserService.DAL;

public sealed class ApplicationDbContext : DbContext
{
    private readonly ILogger _logger;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger logger) : base(options)
    {
        _logger = logger;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(_logger.Information, LogLevel.Information);
        optionsBuilder.AddInterceptors(new DateInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}