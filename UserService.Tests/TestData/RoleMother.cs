using UserService.Domain.Entities;

namespace UserService.Tests.TestData;

internal static class RoleMother
{
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

    public static Role GetRoleUser()
    {
        return new Role
        {
            Id = 1,
            Name = "User"
        };
    }

    public static Role GetRoleAdmin()
    {
        return new Role
        {
            Id = 2,
            Name = "Admin"
        };
    }

    public static Role GetRoleModer()
    {
        return new Role
        {
            Id = 3,
            Name = "Moderator"
        };
    }
}
