using System.Reflection;
using System.Runtime.CompilerServices;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types.Descriptors;
using Microsoft.Extensions.Options;
using QuestionService.Domain.Dtos.Page;
using UserService.Application.Resources;
using UserService.Application.Settings;
using UserService.Domain.Interfaces.Validation;
using UserService.GraphQl.Extensions;
using UserService.GraphQl.Helpers;

namespace UserService.GraphQl.Middlewares;

public class CursorPagingValidationMiddleware(FieldDelegate next)
{
    private const string BeforeArgName = "before";
    private const string AfterArgName = "after";
    private const string FirstArgName = "first";
    private const string LastArgName = "last";
    private const string OrderArgName = "order";

    public async Task InvokeAsync(IMiddlewareContext context,
        INullSafeValidator<CursorPageDto> cursorPageValidator,
        IOptions<PaginationRules> paginationRules)
    {
        var first = context.ArgumentValue<int?>(FirstArgName);
        var after = context.ArgumentValue<string?>(AfterArgName);
        var before = context.ArgumentValue<string?>(BeforeArgName);
        var last = context.ArgumentValue<int?>(LastArgName);
        var order = GetOrderArg(context);

        // Specifying default values if need
        if (after == null && first == null && before == null && last == null)
            first = paginationRules.Value.DefaultPageSize;
        if (after != null && first == null)
            first = paginationRules.Value.DefaultPageSize;
        if (before != null && last == null)
            last = paginationRules.Value.DefaultPageSize;

        var pagination =
            new CursorPageDto(first, after, before, last, order.ToOrderDto());

        if (!cursorPageValidator.IsValid(pagination, out var errors))
            throw GraphQlExceptionHelper.GetException(
                $"{ErrorMessage.InvalidPagination}: {string.Join(' ', errors)}");

        await next(context);
    }

    private static ListValueNode? GetOrderArg(IMiddlewareContext context)
    {
        try
        {
            // If no exception, order argument is null
            context.ArgumentLiteral<NullValueNode>(OrderArgName);
            return null;
        }
        catch (GraphQLException)
        {
            return context.ArgumentLiteral<ListValueNode>(OrderArgName);
        }
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class UseCursorPagingValidationMiddlewareAttribute : ObjectFieldDescriptorAttribute
{
    public UseCursorPagingValidationMiddlewareAttribute([CallerLineNumber] int order = 0)
    {
        Order = order;
    }

    protected override void OnConfigure(IDescriptorContext context,
        IObjectFieldDescriptor descriptor, MemberInfo member)
    {
        descriptor.Use<CursorPagingValidationMiddleware>();
    }
}