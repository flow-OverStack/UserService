using Microsoft.EntityFrameworkCore;

namespace UserService.Tests.FunctionalTests.Configurations.Keycloak;

public class KeycloakDbContext : DbContext
{
    public KeycloakDbContext(DbContextOptions<KeycloakDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KeycloakUser>().Property(x => x.Id).IsRequired();
        modelBuilder.Entity<KeycloakUser>().Property(x => x.Username).IsRequired();
        modelBuilder.Entity<KeycloakUser>().Property(x => x.Password).IsRequired();
    }
}