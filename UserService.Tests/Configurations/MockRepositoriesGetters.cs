using MockQueryable.Moq;
using Moq;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Database;
using UserService.Domain.Interfaces.Repository;
using UserService.Messaging.Events;

namespace UserService.Tests.Configurations;

internal static class MockRepositoriesGetters
{
    public const int MinReputation = 1;
    public const int MaxDailyReputation = 200;

    private static Role GetRoleUser()
    {
        return new Role
        {
            Id = 1,
            Name = "User"
        };
    }

    private static Role GetRoleAdmin()
    {
        return new Role
        {
            Id = 2,
            Name = "Admin"
        };
    }

    private static Role GetRoleModer()
    {
        return new Role
        {
            Id = 3,
            Name = "Moderator"
        };
    }

    private static IMock<ITransaction> GetMockTransaction()
    {
        return new Mock<ITransaction>();
    }

    public static IMock<IUnitOfWork> GetMockUnitOfWork(IBaseRepository<User>? userRepository = null,
        IBaseRepository<Role>? roleRepository = null,
        IBaseRepository<ReputationRecord>? reputationRecordRepository = null,
        IBaseRepository<ReputationRule>? reputationRuleRepository = null)
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUnitOfWork.Setup(x => x.Users).Returns(userRepository ?? GetMockUserRepository().Object);
        mockUnitOfWork.Setup(x => x.Roles).Returns(roleRepository ?? GetMockRoleRepository().Object);
        mockUnitOfWork.Setup(x => x.ReputationRecords)
            .Returns(reputationRecordRepository ?? GetMockReputationRecordRepository().Object);
        mockUnitOfWork.Setup(x => x.ReputationRules)
            .Returns(reputationRuleRepository ?? GetMockReputationRuleRepository().Object);
        mockUnitOfWork.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetMockTransaction().Object);

        return mockUnitOfWork;
    }

    public static IMock<IBaseRepository<User>> GetMockUserRepository()
    {
        var mockRepository = new Mock<IBaseRepository<User>>();
        var users = GetUsers().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(users.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken _) => user);
        mockRepository.Setup(x => x.Update(It.IsAny<User>())).Returns((User user) => user);
        mockRepository.Setup(x => x.Remove(It.IsAny<User>())).Returns((User user) => user);

        return mockRepository;
    }

    public static IMock<IBaseRepository<Role>> GetMockRoleRepository()
    {
        var mockRepository = new Mock<IBaseRepository<Role>>();

        // Adding users to roles

        var roles = GetRoles().ToList();
        roles.ForEach(x => x.Users = []);
        var users = GetUsers().ToList();
        var userRoles = GetUserRoles().ToList();

        foreach (var userRole in userRoles)
        {
            var user = users.First(x => x.Id == userRole.UserId);
            var role = roles.First(x => x.Id == userRole.RoleId);

            role.Users.Add(user);
        }


        var rolesDbSet = roles.BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(rolesDbSet.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role role, CancellationToken _) => role);
        mockRepository.Setup(x => x.Update(It.IsAny<Role>())).Returns((Role role) => role);
        mockRepository.Setup(x => x.Remove(It.IsAny<Role>())).Returns((Role role) => role);

        return mockRepository;
    }

    public static IMock<IBaseRepository<ReputationRule>> GetMockReputationRuleRepository()
    {
        var mockRepository = new Mock<IBaseRepository<ReputationRule>>();
        var rules = GetReputationRules().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(rules.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<ReputationRule>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReputationRule rule, CancellationToken _) => rule);
        mockRepository.Setup(x => x.Update(It.IsAny<ReputationRule>())).Returns((ReputationRule rule) => rule);
        mockRepository.Setup(x => x.Remove(It.IsAny<ReputationRule>())).Returns((ReputationRule rule) => rule);

        return mockRepository;
    }

    public static IMock<IBaseRepository<ReputationRecord>> GetMockReputationRecordRepository()
    {
        var mockRepository = new Mock<IBaseRepository<ReputationRecord>>();
        var records = GetReputationRecords().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(records.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<ReputationRecord>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReputationRecord record, CancellationToken _) => record);
        mockRepository.Setup(x => x.Update(It.IsAny<ReputationRecord>())).Returns((ReputationRecord record) => record);
        mockRepository.Setup(x => x.Remove(It.IsAny<ReputationRecord>())).Returns((ReputationRecord record) => record);

        return mockRepository;
    }

    public static IMock<IBaseRepository<T>> GetEmptyMockRepository<T>() where T : class
    {
        var mockRepository = new Mock<IBaseRepository<T>>();
        var entities = Array.Empty<T>().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(entities.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((T entity, CancellationToken _) => entity);
        mockRepository.Setup(x => x.Update(It.IsAny<T>())).Returns((T entity) => entity);
        mockRepository.Setup(x => x.Remove(It.IsAny<T>())).Returns((T entity) => entity);

        return mockRepository;
    }

    public static IQueryable<User> GetUsers()
    {
        return new User[]
        {
            new()
            {
                Id = 1,
                IdentityId = Guid.NewGuid().ToString(),
                Username = "testuser1",
                Email = "TestUser1@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                OwnedReputationRecords = GetReputationRecords().Where(x => x.ReputationTargetId == 1).ToList(),
                InitiatedReputationRecords = GetReputationRecords().Where(x => x.InitiatorId == 1).ToList(),
                Roles = [GetRoleUser(), GetRoleAdmin()]
            },
            new()
            {
                Id = 2,
                IdentityId = Guid.NewGuid().ToString(),
                Username = "testuser2",
                Email = "TestUser2@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                OwnedReputationRecords = GetReputationRecords().Where(x => x.ReputationTargetId == 2).ToList(),
                InitiatedReputationRecords = GetReputationRecords().Where(x => x.InitiatorId == 2).ToList(),
                Roles = [GetRoleUser(), GetRoleModer()]
            },
            new()
            {
                Id = 3,
                IdentityId = Guid.NewGuid().ToString(),
                Username = "testuser3",
                Email = "TestUser3@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                OwnedReputationRecords = GetReputationRecords().Where(x => x.ReputationTargetId == 3).ToList(),
                InitiatedReputationRecords = GetReputationRecords().Where(x => x.InitiatorId == 3).ToList(),
                Roles = [GetRoleModer()]
            },
            new() //user without roles
            {
                Id = 5, //id is not 4 because 4 is used to create a new user
                IdentityId = Guid.NewGuid().ToString(),
                Username = "testuser5",
                Email = "TestUser5@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                OwnedReputationRecords = GetReputationRecords().Where(x => x.ReputationTargetId == 5).ToList(),
                InitiatedReputationRecords = GetReputationRecords().Where(x => x.InitiatorId == 5).ToList(),
                Roles = []
            }
        }.AsQueryable();
    }

    public static IQueryable<Role> GetRoles()
    {
        return new[] { GetRoleUser(), GetRoleAdmin(), GetRoleModer() }.AsQueryable();
    }

    public static IQueryable<UserRole> GetUserRoles()
    {
        return new UserRole[]
        {
            new()
            {
                RoleId = 1,
                UserId = 1
            },
            new()
            {
                RoleId = 2,
                UserId = 1
            },
            new()
            {
                RoleId = 1,
                UserId = 2
            },
            new()
            {
                RoleId = 3,
                UserId = 2
            },
            new()
            {
                RoleId = 3,
                UserId = 3
            }
        }.AsQueryable();
    }

    public static IQueryable<ProcessedEvent> GetProcessedEvents()
    {
        return new[]
        {
            new ProcessedEvent
            {
                EventId = Guid.NewGuid(),
                ProcessedAt = DateTime.UtcNow
            },
            new ProcessedEvent
            {
                EventId = Guid.NewGuid(),
                ProcessedAt = DateTime.UtcNow.AddDays(-7)
            },
            new ProcessedEvent
            {
                EventId = Guid.NewGuid(),
                ProcessedAt = DateTime.UtcNow.AddDays(-7)
            }
        }.AsQueryable();
    }

    public static IQueryable<ReputationRule> GetReputationRules()
    {
        return new[]
        {
            new ReputationRule
            {
                Id = 1, EventType = nameof(BaseEventType.EntityAccepted), EntityType = nameof(EntityType.Answer),
                Group = null, ReputationChange = 15, ReputationTarget = ReputationTarget.Author
            },
            new ReputationRule
            {
                Id = 2, EventType = nameof(BaseEventType.EntityDownvoted),
                ReputationTarget = ReputationTarget.Initiator, EntityType = nameof(EntityType.Answer), Group = null,
                ReputationChange = -1
            },
            new ReputationRule
            {
                Id = 3, EventType = nameof(BaseEventType.EntityDownvoted), EntityType = nameof(EntityType.Answer),
                Group = "Vote", ReputationChange = -2, ReputationTarget = ReputationTarget.Author
            },
            new ReputationRule
            {
                Id = 4, EventType = nameof(BaseEventType.EntityUpvoted), EntityType = nameof(EntityType.Answer),
                Group = "Vote", ReputationChange = 10, ReputationTarget = ReputationTarget.Author
            },
            new ReputationRule
            {
                Id = 5, EventType = nameof(BaseEventType.EntityAccepted), EntityType = nameof(EntityType.Answer),
                Group = null, ReputationChange = 2, ReputationTarget = ReputationTarget.Initiator
            },
            new ReputationRule
            {
                Id = 6, EventType = nameof(BaseEventType.EntityDownvoted), EntityType = nameof(EntityType.Question),
                Group = "Vote", ReputationChange = -2, ReputationTarget = ReputationTarget.Author
            },
            new ReputationRule
            {
                Id = 7, EventType = nameof(BaseEventType.EntityUpvoted), EntityType = nameof(EntityType.Question),
                Group = "Vote", ReputationChange = 10, ReputationTarget = ReputationTarget.Author
            },
            new ReputationRule
            {
                Id = 8, EventType = "TestSuperEvent", EntityType = nameof(EntityType.Question),
                Group = null, ReputationChange = MaxDailyReputation, ReputationTarget = ReputationTarget.Author
            }
        }.AsQueryable();
    }

    public static IQueryable<ReputationRecord> GetReputationRecords()
    {
        var rules = GetReputationRules().ToList();

        var ruleAnswerAccepted = rules.First(x => x.Id == 1);
        var ruleAnswerDownvoteInitiator = rules.First(x => x.Id == 2);
        var ruleAnswerDownvote = rules.First(x => x.Id == 3);
        var ruleAnswerUpvote = rules.First(x => x.Id == 4);
        var ruleAnswerAcceptedInitiator = rules.First(x => x.Id == 5);
        var ruleQuestionDownvote = rules.First(x => x.Id == 6);
        var ruleQuestionUpvote = rules.First(x => x.Id == 7);
        var superRule = rules.First(x => x.Id == 8);

        return new[]
        {
            new ReputationRecord
            {
                Id = 1,
                ReputationTargetId = 2,
                InitiatorId = 1,
                EntityId = 1,
                ReputationRule = ruleQuestionDownvote,
                ReputationRuleId = ruleQuestionDownvote.Id,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new ReputationRecord
            {
                Id = 2,
                ReputationTargetId = 3,
                InitiatorId = 1,
                EntityId = 1,
                ReputationRule = ruleAnswerDownvote,
                ReputationRuleId = ruleAnswerDownvote.Id,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new ReputationRecord
            {
                Id = 3,
                ReputationTargetId = 1,
                InitiatorId = 1,
                EntityId = 1,
                ReputationRule = ruleAnswerDownvoteInitiator,
                ReputationRuleId = ruleAnswerDownvoteInitiator.Id,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new ReputationRecord
            {
                Id = 4,
                ReputationTargetId = 3,
                InitiatorId = 2,
                EntityId = 2,
                ReputationRule = ruleQuestionUpvote,
                ReputationRuleId = ruleQuestionUpvote.Id,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new ReputationRecord
            {
                Id = 5,
                ReputationTargetId = 1,
                InitiatorId = 2,
                EntityId = 2,
                ReputationRule = ruleAnswerUpvote,
                ReputationRuleId = ruleAnswerUpvote.Id,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new ReputationRecord
            {
                Id = 6,
                ReputationTargetId = 1,
                InitiatorId = 2,
                EntityId = 2,
                ReputationRule = ruleAnswerAccepted,
                ReputationRuleId = ruleAnswerAccepted.Id,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new ReputationRecord
            {
                Id = 7,
                ReputationTargetId = 2,
                InitiatorId = 2,
                EntityId = 2,
                ReputationRule = ruleAnswerAcceptedInitiator,
                ReputationRuleId = ruleAnswerAcceptedInitiator.Id,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new ReputationRecord
            {
                Id = 8,
                ReputationTargetId = 2,
                InitiatorId = 3,
                EntityId = 3,
                ReputationRuleId = ruleAnswerUpvote.Id,
                ReputationRule = ruleAnswerUpvote,
                Enabled = true,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new ReputationRecord
            {
                Id = 9,
                ReputationTargetId = 3,
                InitiatorId = 3,
                EntityId = 3,
                ReputationRuleId = superRule.Id,
                ReputationRule = superRule,
                Enabled = true,
                CreatedAt = DateTime.UtcNow
            },
            new ReputationRecord
            {
                Id = 10,
                ReputationTargetId = 2,
                InitiatorId = 3,
                EntityId = 4,
                ReputationRuleId = ruleAnswerUpvote.Id,
                ReputationRule = ruleAnswerUpvote,
                Enabled = false,
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            }
        }.AsQueryable();
    }
}