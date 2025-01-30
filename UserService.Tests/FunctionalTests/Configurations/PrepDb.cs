using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserService.DAL;
using UserService.Domain.Entity;
using UserService.Tests.Configurations;
using UserService.Tests.FunctionalTests.Configurations.Keycloak;

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
            KeycloakId = x.KeycloakId,
            Username = x.Username,
            Email = x.Email,
            Reputation = x.Reputation,
            CreatedAt = x.CreatedAt,
            LastLoginAt = x.LastLoginAt
        });
        var userRoles = MockRepositoriesGetters.GetUserRoles();
        var userTokens = MockRepositoriesGetters.GetUserTokens().Select(x => new UserToken
        {
            RefreshToken = x.RefreshToken,
            RefreshTokenExpiryTime = x.RefreshTokenExpiryTime,
            UserId = x.UserId
        });

        dbContext.Set<User>().AddRange(users);
        dbContext.Set<UserRole>().AddRange(userRoles);
        dbContext.Set<UserToken>().AddRange(userTokens);

        dbContext.SaveChanges();

        PrepareKeycloak(services, users);
    }

    private static void PrepareKeycloak(this IServiceCollection services, IEnumerable<User> users)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<KeycloakDbContext>();

        dbContext.Database.EnsureCreated();

        var keycloakUsers = users.Select((x, index) => new KeycloakUser
        {
            Id = Guid.NewGuid(),
            Username = x.Username,
            Password = TestConstants.TestPassword + (index + 1) //e.g. TestPassword1
        });

        dbContext.Set<KeycloakUser>().AddRange(keycloakUsers);

        dbContext.SaveChanges();
    }
}