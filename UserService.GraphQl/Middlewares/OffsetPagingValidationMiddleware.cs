using System.Reflection;
using System.Runtime.CompilerServices;
using FluentValidation;
using HotChocolate.Resolvers;
using HotChocolate.Types.Descriptors;
using Microsoft.Extensions.Options;
using UserService.Application.Resources;
using UserService.Application.Settings;
using UserService.Domain.Dtos.Page;
using UserService.GraphQl.Helpers;

namespace UserService.GraphQl.Middlewares;

public class OffsetPagingValidationMiddleware(FieldDelegate next)
{
    private const string SkipArgName = "skip";
    private const string TakeArgName = "take";

    public async Task InvokeAsync(IMiddlewareContext context,
        IValidator<OffsetPageDto> offsetPageValidator,
        IOptions<PaginationRules> paginationRules)
    {
        var skip = context.ArgumentValue<int?>(SkipArgName) ?? 0; // Value by default
        var take = context.ArgumentValue<int?>(TakeArgName) ??
                   paginationRules.Value.DefaultPageSize; // Value by default

        var pagination = new OffsetPageDto(skip, take);

        var validation = await offsetPageValidator.ValidateAsync(pagination, context.RequestAborted);
        if (!validation.IsValid)
            throw GraphQlExceptionHelper.GetException(
                $"{ErrorMessage.InvalidPagination}: {string.Join(' ', validation.Errors)}");

        await next(context);
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class UseOffsetPagingValidationMiddlewareAttribute : ObjectFieldDescriptorAttribute
{
    public UseOffsetPagingValidationMiddlewareAttribute([CallerLineNumber] int order = 0)
    {
        Order = order;
    }

    protected override void OnConfigure(IDescriptorContext context,
        IObjectFieldDescriptor descriptor, MemberInfo member)
    {
        descriptor.Use<OffsetPagingValidationMiddleware>();
    }
}