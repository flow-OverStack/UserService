using MockQueryable.Moq;
using Moq;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Database;
using UserService.Domain.Interfaces.Repository;
using UserService.Tests.TestData;

namespace UserService.Tests.Mocks;

internal static class RepositoryMocks
{
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
        var users = UserMother.GetUsers().BuildMockDbSet();

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

        var roles = RoleMother.GetRoles().ToList();
        roles.ForEach(x => x.Users = []);
        var users = UserMother.GetUsers().ToList();
        var userRoles = RoleMother.GetUserRoles().ToList();

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
        var rules = ReputationRuleMother.GetReputationRules().BuildMockDbSet();

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
        var records = ReputationRecordMother.GetReputationRecords().BuildMockDbSet();

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
}
