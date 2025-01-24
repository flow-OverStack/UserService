using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserService.DAL;
using UserService.Domain.Entity;
using UserService.Tests.UnitTests.Configurations;

namespace UserService.Tests.Extensions;

internal static class PrepDb
{
    internal static void PrepPopulation(this IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();

        var users = MockRepositoriesGetters.GetUsers().Select(x => new User
        {
            Id = x.Id,
            KeycloakId = x.KeycloakId,
            Username = x.Username,
            Email = x.Email,
            Reputation = x.Reputation,
            CreatedAt = x.CreatedAt,
            LastLoginAt = x.LastLoginAt
        });
        var roles = MockRepositoriesGetters.GetRoles();
        var userRoles = MockRepositoriesGetters.GetUserRoles();
        var userTokens = MockRepositoriesGetters.GetUserTokens();

        dbContext.Set<User>().AddRange(users);
        dbContext.Set<Role>().AddRange(roles);
        dbContext.Set<UserRole>().AddRange(userRoles);
        dbContext.Set<UserToken>().AddRange(userTokens);

        dbContext.SaveChanges();
    }
}