using UserService.Domain.Dtos.Request.Grpahql;
using UserService.Domain.Dtos.Request.Page;
using UserService.Domain.Entities;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Service;
using UserService.GraphQl.DataLoaders;

namespace UserService.GraphQl;

public class Queries
{
    [GraphQLDescription("Returns a list of paginated users.")]
    [UseFiltering]
    [UseSorting]
    public async Task<PaginatedResult<User>> GetUsers(PageDto pagination, [Service] IGetUserService userService,
        CancellationToken cancellationToken)
    {
        var result = await userService.GetAllAsync(pagination, cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return new PaginatedResult<User>(result, pagination.PageSize);
    }

    [GraphQLDescription("Returns a user by its id.")]
    [UseFiltering]
    [UseSorting]
    public async Task<User?> GetUser(long id, UserDataLoader userLoader, CancellationToken cancellationToken)
    {
        var user = await userLoader.LoadAsync(id, cancellationToken);

        return user;
    }


    [GraphQLDescription("Returns a list of paginated roles.")]
    [UseFiltering]
    [UseSorting]
    public async Task<PaginatedResult<Role>> GetRoles(PageDto pagination, [Service] IGetRoleService roleService,
        CancellationToken cancellationToken)
    {
        var result = await roleService.GetAllAsync(pagination, cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return new PaginatedResult<Role>(result, pagination.PageSize);
    }

    [GraphQLDescription("Returns a role by its id.")]
    [UseFiltering]
    [UseSorting]
    public async Task<Role?> GetRole(long id, RoleDataLoader roleLoader, CancellationToken cancellationToken)
    {
        var role = await roleLoader.LoadAsync(id, cancellationToken);

        return role;
    }
}