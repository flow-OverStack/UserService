using UserService.Domain.Entity;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Services;

namespace UserService.GraphQl.Types;

public class RoleType : ObjectType<Role>
{
    protected override void Configure(IObjectTypeDescriptor<Role> descriptor)
    {
        descriptor.Description("The role type.");
        descriptor.Field(x => x.Id).Description("The ID of the role.");
        descriptor.Field(x => x.Name).Description("The name of the role.");
        descriptor.Field(x => x.Users).Description("Users who have the role.");
        descriptor.Field(x => x.GetRoleName()).Ignore();

        descriptor.Field(x => x.Users).ResolveWith<Resolvers>(x => x.GetUsersAsync(default!, default!));
    }

    private sealed class Resolvers
    {
        public async Task<IEnumerable<User>> GetUsersAsync([Parent] Role role, [Service] IGetUserService userService)
        {
            var result = await userService.GetUsersWithRole(role.Id);

            if (!result.IsSuccess)
                throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

            return result.Data;
        }
    }
}