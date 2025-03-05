using UserService.Domain.Entity;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Result;

namespace UserService.GraphQl;

public class Queries
{
    [GraphQLDescription("Returns a list of all users")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<User>> GetUsers([Service] IGraphQlService graphQlService)
    {
        CollectionResult<User> result;
        try
        {
            result = await graphQlService.GetAllUsersAsync();
        }
        catch (Exception e)
        {
            throw GraphQlExceptionHelper.GetInternal(e);
        }

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.Get(result.ErrorMessage!);

        return result.Data;
    }


    [GraphQLDescription("Returns a list of all roles")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<Role>> GetRoles([Service] IGraphQlService graphQlService)
    {
        CollectionResult<Role> result;
        try
        {
            result = await graphQlService.GetAllRolesAsync();
        }
        catch (Exception e)
        {
            throw GraphQlExceptionHelper.GetInternal(e);
        }

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.Get(result.ErrorMessage!);

        return result.Data;
    }
}