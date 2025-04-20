using UserService.Domain.Entity;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Services;
using UserService.GraphQl.DataLoaders;

namespace UserService.GraphQl;

public class Queries
{
    [GraphQLDescription("Returns a list of all users")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<User>> GetUsers([Service] IGetUserService userService)
    {
        var result = await userService.GetAllAsync();

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a user by its id. If one is requested - error, several - null")]
    [UseFiltering]
    [UseSorting]
    public async Task<User?> GetUser(long id, UserDataLoader userLoader)
    {
        //If one user is not found, data loader will throw GrpahQl exception
        var user = await userLoader.LoadAsync(id);

        return user;
    }


    [GraphQLDescription("Returns a list of all roles")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<Role>> GetRoles([Service] IGetRoleService roleService)
    {
        var result = await roleService.GetAllAsync();

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a role by its id. If one is requested - error, several - null")]
    [UseFiltering]
    [UseSorting]
    public async Task<Role?> GetRole(long id, RoleDataLoader roleLoader)
    {
        //If one role is not found, data loader will throw GrpahQl exception
        var role = await roleLoader.LoadAsync(id);

        return role;
    }
}