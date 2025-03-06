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
        var result = await userService.GetAllUsersAsync();

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.Get(result.ErrorMessage!);

        return result.Data;
    }


    [GraphQLDescription("Returns a list of all roles")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<Role>> GetRoles([Service] IGetRoleService roleService)
    {
        var result = await roleService.GetAllRolesAsync();

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.Get(result.ErrorMessage!);

        return result.Data;
    }
}