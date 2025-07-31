using HotChocolate.ApolloFederation.Types;
using UserService.Application.Resources;
using UserService.Domain.Entities;
using UserService.Domain.Extensions;
using UserService.GraphQl.DataLoaders;
using UserService.GraphQl.Helpers;

namespace UserService.GraphQl.Types;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Description("The user type.");
        descriptor.Field(x => x.Id).Description("The ID of the user.");
        descriptor.Field(x => x.IdentityId).Description("The ID of the user in identity server.");
        descriptor.Field(x => x.Username).Description("The name of the user.");
        descriptor.Field(x => x.Email).Description("The email address of the user.");
        descriptor.Field(x => x.LastLoginAt).Description("The last login time of the user.");
        descriptor.Field(x => x.Reputation).Description("The reputation of the user.");
        descriptor.Field(x => x.Roles).Description("The roles of the user.");
        descriptor.Field(x => x.CreatedAt).Description("User creation time.");

        descriptor.Field(x => x.Roles).ResolveWith<Resolvers>(x => x.GetRolesAsync(default!, default!, default!));

        descriptor.Key(nameof(User.Id).LowercaseFirstLetter())
            .ResolveReferenceWith(_ => Resolvers.GetUserByIdAsync(default!, default!, default!));
    }

    private sealed class Resolvers
    {
        public async Task<IEnumerable<Role>> GetRolesAsync([Parent] User user, GroupRoleDataLoader roleLoader,
            CancellationToken cancellationToken)
        {
            var roles = await roleLoader.LoadRequiredAsync(user.Id, cancellationToken);

            // Having no roles is a business exception, so we got to check it here
            if (roles.Length == 0)
                throw GraphQlExceptionHelper.GetException(ErrorMessage.RolesNotFound);

            return roles;
        }

        public static async Task<User?> GetUserByIdAsync(long id, UserDataLoader userLoader,
            CancellationToken cancellationToken)
        {
            var user = await userLoader.LoadAsync(id, cancellationToken);

            return user;
        }
    }
}