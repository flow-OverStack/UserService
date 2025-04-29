using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.DAL.Interceptors;
using UserService.Domain.Entities;
using UserService.Domain.Settings;
using ILogger = Serilog.ILogger;

namespace UserService.DAL;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ILogger logger,
    IOptions<BusinessRules> businessRules)
    : DbContext(options)
{
    private readonly BusinessRules _businessRules = businessRules.Value;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(logger.Information, LogLevel.Information);
        optionsBuilder.AddInterceptors(new DateInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        ApplyReputationRules(modelBuilder);
    }

    private void ApplyReputationRules(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(x => x.Reputation).IsRequired().HasDefaultValue(_businessRules.MinReputation);

        //Reputation constraint
        modelBuilder.Entity<User>()
            .ToTable(t => t.HasCheckConstraint("CK_User_Reputation", $"""
                                                                      "Reputation" >= {_businessRules.MinReputation}
                                                                      """));

        //ReputationEarnedToday constraint
        modelBuilder.Entity<User>()
            .ToTable(t => t.HasCheckConstraint("CK_User_ReputationEarnedToday", $"""
                 "ReputationEarnedToday" >= 0 AND "ReputationEarnedToday" <= {_businessRules.MaxDailyReputation}
                 """));
    }
}