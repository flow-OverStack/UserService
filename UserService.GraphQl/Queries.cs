using UserService.Domain.Entity;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Services;

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

    [GraphQLDescription("Returns a user by its id")]
    [UseFiltering]
    [UseSorting]
    public async Task<User> GetUser(long id, [Service] IGetUserService userService)
    {
        var result = await userService.GetByIdAsync(id);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
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

    [GraphQLDescription("Returns a role by its id")]
    [UseFiltering]
    [UseSorting]
    public async Task<Role> GetRole(long id, [Service] IGetRoleService roleService)
    {
        var result = await roleService.GetByIdAsync(id);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }
}