using UserService.Domain.Entities;

namespace UserService.Tests.TestData;

internal static class UserMother
{
    public static IQueryable<User> GetUsers()
    {
        return new User[]
        {
            new()
            {
                Id = 1,
                IdentityId = "test-identity-id-1",
                Username = "testuser1",
                Email = "TestUser1@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                OwnedReputationRecords = ReputationRecordMother.GetReputationRecords()
                    .Where(x => x.ReputationTargetId == 1).ToList(),
                InitiatedReputationRecords = ReputationRecordMother.GetReputationRecords()
                    .Where(x => x.InitiatorId == 1).ToList(),
                Roles = [RoleMother.GetRoleUser(), RoleMother.GetRoleAdmin()]
            },
            new()
            {
                Id = 2,
                IdentityId = Guid.NewGuid().ToString(),
                Username = "testuser2",
                Email = "TestUser2@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                OwnedReputationRecords = ReputationRecordMother.GetReputationRecords()
                    .Where(x => x.ReputationTargetId == 2).ToList(),
                InitiatedReputationRecords = ReputationRecordMother.GetReputationRecords()
                    .Where(x => x.InitiatorId == 2).ToList(),
                Roles = [RoleMother.GetRoleUser(), RoleMother.GetRoleModer()]
            },
            new()
            {
                Id = 3,
                IdentityId = Guid.NewGuid().ToString(),
                Username = "testuser3",
                Email = "TestUser3@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                OwnedReputationRecords = ReputationRecordMother.GetReputationRecords()
                    .Where(x => x.ReputationTargetId == 3).ToList(),
                InitiatedReputationRecords = ReputationRecordMother.GetReputationRecords()
                    .Where(x => x.InitiatorId == 3).ToList(),
                Roles = [RoleMother.GetRoleModer()]
            },
            new() //user without roles
            {
                Id = 5, //id is not 4 because 4 is used to create a new user
                IdentityId = Guid.NewGuid().ToString(),
                Username = "testuser5",
                Email = "TestUser5@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                OwnedReputationRecords = ReputationRecordMother.GetReputationRecords()
                    .Where(x => x.ReputationTargetId == 5).ToList(),
                InitiatedReputationRecords = ReputationRecordMother.GetReputationRecords()
                    .Where(x => x.InitiatorId == 5).ToList(),
                Roles = []
            }
        }.AsQueryable();
    }
}
