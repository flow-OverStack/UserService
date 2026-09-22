using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserService.DAL;
using UserService.Domain.Entities;
using UserService.Messaging.Events;
using UserService.Tests.Constants;
using UserService.Tests.FunctionalTests.Configurations.Keycloak;
using UserService.Tests.TestData;

namespace UserService.Tests.FunctionalTests.Configurations;

internal static class PrepDb
{
    public static void PrepPopulation(this IServiceScope serviceScope)
    {
        var users = UserMother.GetUsers()
            //Real user always has at least 1 role
            .Where(x => x.Roles.Count >= 1)
            .Select(x => new User
            {
                IdentityId = x.IdentityId,
                Username = x.Username,
                Email = x.Email,
                CreatedAt = x.CreatedAt,
                LastLoginAt = x.LastLoginAt
            });

        serviceScope.PrepAppDb(users);

        serviceScope.PrepKeycloakDb(users);
    }

    private static void PrepAppDb(this IServiceScope serviceScope, IEnumerable<User> users)
    {
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();

        var roles = RoleMother.GetRoles();
        var userRoles = RoleMother.GetUserRoles();
        var processedEvents = ProcessedEventMother.GetProcessedEvents();
        var reputationRules = ReputationRuleMother.GetReputationRules();
        var reputationRecords = ReputationRecordMother.GetReputationRecords();

        roles.ToList().ForEach(x => x.Id = 0);
        reputationRecords.ToList().ForEach(x =>
        {
            x.Id = 0;
            x.ReputationRule = null!;
        });

        dbContext.Set<Role>().AddRange(roles);
        dbContext.SaveChanges();

        dbContext.Set<User>().AddRange(users);
        dbContext.Set<UserRole>().AddRange(userRoles);
        dbContext.Set<ProcessedEvent>().AddRange(processedEvents);
        dbContext.Set<ReputationRule>().AddRange(reputationRules);
        dbContext.SaveChanges();

        dbContext.Set<ReputationRecord>().AddRange(reputationRecords);
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
                Email = x.Email,
                Password = TestConstants.TestPassword + (index + 1) //e.g. TestPassword1
            })
            .Prepend(new KeycloakUser
            {
                Id = Guid.NewGuid(),
                Username = TestConstants.ExistingUsername,
                Email = TestConstants.ExistingUsername + "@test.com",
                Password = TestConstants.TestPassword + TestConstants.ExistingUsername
            });

        dbContext.Set<KeycloakUser>().AddRange(keycloakUsers);

        dbContext.SaveChanges();
    }
}