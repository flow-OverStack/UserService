using HotChocolate;
using UserService.Domain.Entity;
using UserService.Domain.Exceptions;
using UserService.Domain.Interfaces.Services;

namespace UserService.GraphQl;

public class Queries
{
    [GraphQLDescription("Returns a list of all users")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<User>> GetUsers([Service] IGraphQlService graphQlService)
    {
        try
        {
            var result = await graphQlService.GetAllUsersAsync();
            if (!result.IsSuccess)
                throw new GraphQlException(result.ErrorMessage!);

            return result.Data;
        }
        catch (Exception e)
        {
            throw GraphQlException.Internal(e);
        }
    }

    [GraphQLDescription("Returns a list of all roles")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<Role>> GetRoles([Service] IGraphQlService graphQlService)
    {
        try
        {
            var result = await graphQlService.GetAllRolesAsync();
            if (!result.IsSuccess)
                throw new GraphQlException(result.ErrorMessage!);

            return result.Data;
        }
        catch (Exception e)
        {
            throw GraphQlException.Internal(e);
        }
    }
}