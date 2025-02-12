using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserService.DAL;
using UserService.Domain.Entity;
using UserService.Tests.Configurations;
using UserService.Tests.Constants;
using UserService.Tests.FunctionalTests.Configurations.Keycloak;

namespace UserService.Tests.FunctionalTests.Configurations;

internal static class PrepDb
{
    public static void PrepPopulation(this IServiceScope serviceScope)
    {
        var users = MockRepositoriesGetters.GetUsers().Select(x => new User
        {
            KeycloakId = x.KeycloakId,
            Username = x.Username,
            Email = x.Email,
            Reputation = x.Reputation,
            CreatedAt = x.CreatedAt,
            LastLoginAt = x.LastLoginAt
        });

        PrepAppDb(serviceScope, users);

        PrepKeycloakDb(serviceScope, users);
    }

    private static void PrepAppDb(this IServiceScope serviceScope, IEnumerable<User> users)
    {
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();


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
    }

    private static void PrepKeycloakDb(this IServiceScope serviceScope, IEnumerable<User> users)
    {
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<KeycloakDbContext>();

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