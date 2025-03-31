using HotChocolate.ApolloFederation.Types;
using UserService.Domain.Entity;
using UserService.Domain.Extensions;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Services;
using UserService.GraphQl.DataLoaders;

namespace UserService.GraphQl.Types;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Description("The user type.");
        descriptor.Field(x => x.Id).Description("The ID of the user.");
        descriptor.Field(x => x.KeycloakId).Description("The ID of the user in keycloak identity server.");
        descriptor.Field(x => x.Username).Description("The name of the user.");
        descriptor.Field(x => x.Email).Description("The email address of the user.");
        descriptor.Field(x => x.LastLoginAt).Description("The last login time of the user.");
        descriptor.Field(x => x.Reputation).Description("The reputation of the user.");
        descriptor.Field(x => x.Roles).Description("The roles of the user.");
        descriptor.Field(x => x.CreatedAt).Description("User creation time.");

        descriptor.Field(x => x.Roles).ResolveWith<Resolvers>(x => x.GetRolesAsync(default!, default!));

        descriptor.Key(nameof(User.Id).LowercaseFirstLetter())
            .ResolveReferenceWith(_ => Resolvers.GetUserByIdAsync(default!, default!));
    }

    private sealed class Resolvers
    {
        public async Task<IEnumerable<Role>> GetRolesAsync([Parent] User user, [Service] IGetRoleService roleService)
        {
            var result = await roleService.GetUserRoles(user.Id);

            if (!result.IsSuccess)
                throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

            return result.Data;
        }

        public static async Task<User> GetUserByIdAsync(long id, UserDataLoader userLoader)
        {
            var user = await userLoader.LoadRequiredAsync(id);

            return user;
        }
    }
}