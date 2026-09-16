using HotChocolate.Authorization;
using Microsoft.AspNetCore.Http;
using UserService.Domain.Entities;
using UserService.Domain.Extensions;
using UserService.Domain.Interfaces.Service;
using UserService.GraphQl.DataLoaders;
using UserService.GraphQl.Helpers;
using UserService.GraphQl.Middlewares;

namespace UserService.GraphQl;

public class Queries
{
    [GraphQLDescription("Returns the currently authenticated user.")]
    [Authorize]
    public async Task<User?> GetMe([Service] IHttpContextAccessor httpContextAccessor, UserDataLoader userLoader,
        CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user == null)
            throw GraphQlExceptionHelper.GetException("User is not authenticated.");

        var userId = user.GetUserId();

        return await userLoader.LoadAsync(userId, cancellationToken);
    }

    [GraphQLDescription("Returns a list of paginated users.")]
    [UseOffsetPagingValidationMiddleware]
    [UseOffsetPaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> GetUsers([Service] IGetUserService userService)
    {
        var result = userService.GetAll();

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
    public IQueryable<Role> GetRoles([Service] IGetRoleService roleService)
    {
        var result = roleService.GetAll();

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
    public IQueryable<ReputationRecord> GetReputationRecords(
        [Service] IGetReputationRecordService recordService)
    {
        var result = recordService.GetAll();

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
    public IQueryable<ReputationRule> GetReputationRules(
        [Service] IGetReputationRuleService ruleService)
    {
        var result = ruleService.GetAll();

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