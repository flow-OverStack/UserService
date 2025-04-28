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

    private static IMock<IDbContextTransaction> GetMockTransaction()
    {
        return new Mock<IDbContextTransaction>();
    }

    public static IMock<IUnitOfWork> GetMockUnitOfWork()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUnitOfWork.Setup(x => x.Users).Returns(GetMockUserRepository().Object);
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
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role role, CancellationToken _) => role);
        mockRepository.Setup(x => x.Update(It.IsAny<Role>())).Returns((Role role) => role);
        mockRepository.Setup(x => x.Remove(It.IsAny<Role>())).Returns((Role role) => role);

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

    public static IMock<IBaseRepository<UserRole>> GetMockUserRoleRepository()
    {
        var mockRepository = new Mock<IBaseRepository<UserRole>>();
        var userRoles = GetUserRoles().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(userRoles.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserRole userRole, CancellationToken _) => userRole);
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