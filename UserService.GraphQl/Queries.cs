using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Service;
using UserService.GraphQl.DataLoaders;
using UserService.GraphQl.Helpers;
using UserService.GraphQl.Middlewares;

namespace UserService.GraphQl;

public class Queries
{
    [GraphQLDescription("Returns a list of paginated users.")]
    [UseOffsetPagingValidationMiddleware]
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
    [UseOffsetPagingValidationMiddleware]
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

    [GraphQLDescription("Returns a list of paginated reputation records.")]
    [UseCursorPagingValidationMiddleware]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ReputationRecord>> GetReputationRecords(
        [Service] IGetReputationRecordService recordService, CancellationToken cancellationToken)
    {
        var result = await recordService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a reputation record by its id.")]
    [UseFiltering]
    [UseSorting]
    public async Task<ReputationRecord?> GetReputationRecord(long id, ReputationRecordDataLoader recordLoader,
        CancellationToken cancellationToken)
    {
        var record = await recordLoader.LoadAsync(id, cancellationToken);

        return record;
    }

    [GraphQLDescription("Returns a list of paginated reputation rules.")]
    [UseOffsetPagingValidationMiddleware]
    [UseOffsetPaging]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<ReputationRule>> GetReputationRules(
        [Service] IGetReputationRuleService ruleService, CancellationToken cancellationToken)
    {
        var result = await ruleService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a reputation rule by its id.")]
    [UseFiltering]
    [UseSorting]
    public async Task<ReputationRule?> GetReputationRule(long id, ReputationRuleDataLoader ruleLoader,
        CancellationToken cancellationToken)
    {
        var rule = await ruleLoader.LoadAsync(id, cancellationToken);

        return rule;
    }
}