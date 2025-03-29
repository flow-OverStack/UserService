using Microsoft.EntityFrameworkCore.Storage;
using MockQueryable.Moq;
using Moq;
using UserService.Domain.Entity;
using UserService.Domain.Events;
using UserService.Domain.Interfaces.Repositories;

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

    private static Mock<IDbContextTransaction> GetMockTransaction()
    {
        var transaction = new Mock<IDbContextTransaction>();

        transaction.Setup(x => x.CommitAsync(default)).Returns(Task.CompletedTask);
        transaction.Setup(x => x.RollbackAsync(default)).Returns(Task.CompletedTask);

        return transaction;
    }

    public static Mock<IUnitOfWork> GetMockUnitOfWork()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUnitOfWork.Setup(x => x.Users).Returns(GetMockUserRepository().Object);
        mockUnitOfWork.Setup(x => x.UserRoles).Returns(GetMockUserRoleRepository().Object);
        mockUnitOfWork.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(GetMockTransaction().Object);
        mockUnitOfWork.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(-1); //-1 is here to indicate that the method is a mock

        return mockUnitOfWork;
    }

    public static Mock<IBaseRepository<User>> GetMockUserRepository()
    {
        var mockRepository = new Mock<IBaseRepository<User>>();
        var users = GetUsers().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(users.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync((User user) => user);
        mockRepository.Setup(x => x.Update(It.IsAny<User>())).Returns((User user) => user);
        mockRepository.Setup(x => x.Remove(It.IsAny<User>())).Returns((User user) => user);

        return mockRepository;
    }

    public static Mock<IBaseRepository<Role>> GetMockRoleRepository()
    {
        var mockRepository = new Mock<IBaseRepository<Role>>();

        #region Adding users to roles

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

        #endregion

        var rolesDbSet = roles.BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(rolesDbSet.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<Role>())).ReturnsAsync((Role role) => role);
        mockRepository.Setup(x => x.Update(It.IsAny<Role>())).Returns((Role role) => role);
        mockRepository.Setup(x => x.Remove(It.IsAny<Role>())).Returns((Role role) => role);

        return mockRepository;
    }

    public static Mock<IBaseRepository<T>> GetEmptyMockRepository<T>() where T : class
    {
        var mockRepository = new Mock<IBaseRepository<T>>();
        var roles = Array.Empty<T>().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(roles.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<T>())).ReturnsAsync((T role) => role);
        mockRepository.Setup(x => x.Update(It.IsAny<T>())).Returns((T role) => role);
        mockRepository.Setup(x => x.Remove(It.IsAny<T>())).Returns((T role) => role);

        return mockRepository;
    }

    public static Mock<IBaseRepository<UserRole>> GetMockUserRoleRepository()
    {
        var mockRepository = new Mock<IBaseRepository<UserRole>>();
        var userRoles = GetUserRoles().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(userRoles.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<UserRole>())).ReturnsAsync((UserRole userRole) => userRole);
        mockRepository.Setup(x => x.Update(It.IsAny<UserRole>())).Returns((UserRole userRole) => userRole);
        mockRepository.Setup(x => x.Remove(It.IsAny<UserRole>())).Returns((UserRole userRole) => userRole);

        return mockRepository;
    }

    public static IQueryable<User> GetUsers()
    {
        return new User[]
        {
            new()
            {
                Id = 1,
                KeycloakId = Guid.NewGuid(),
                Username = "testuser1",
                Email = "TestUser1@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = MinReputation,
                ReputationEarnedToday = 0,
                Roles = [GetRoleUser(), GetRoleAdmin()]
            },
            new()
            {
                Id = 2,
                KeycloakId = Guid.NewGuid(),
                Username = "testuser2",
                Email = "TestUser2@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = MinReputation,
                ReputationEarnedToday = 0,
                Roles = [GetRoleUser(), GetRoleModer()]
            },
            new()
            {
                Id = 3,
                KeycloakId = Guid.NewGuid(),
                Username = "testuser3",
                Email = "TestUser3@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = 200,
                ReputationEarnedToday = MaxDailyReputation,
                Roles = [GetRoleModer()]
            },
            new() //user without roles
            {
                Id = 5, //id is not 4 because 4 is used to create a new user
                KeycloakId = Guid.NewGuid(),
                Username = "testuser5",
                Email = "TestUser5@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = MinReputation,
                ReputationEarnedToday = 0,
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
}