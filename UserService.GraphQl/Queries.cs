using UserService.Domain.Entities;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Service;
using UserService.GraphQl.DataLoaders;

namespace UserService.GraphQl;

public class Queries
{
    [GraphQLDescription("Returns a list of paginated users.")]
    // Page size is validated in the PagingValidationMiddleware
    [UseOffsetPaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<User>> GetUsers([Service] IGetUserService userService,
        CancellationToken cancellationToken)
    {
        var result = await userService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
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
    // Page size is validated in the PagingValidationMiddleware
    [UseOffsetPaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Role>> GetRoles([Service] IGetRoleService roleService,
        CancellationToken cancellationToken)
    {
        var result = await roleService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
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