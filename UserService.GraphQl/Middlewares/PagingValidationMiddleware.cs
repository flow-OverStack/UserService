using HotChocolate.Resolvers;
using Microsoft.Extensions.Options;
using UserService.Domain.Dtos.Request.Page;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Validation;
using UserService.Domain.Resources;
using UserService.Domain.Settings;

namespace UserService.GraphQl.Middlewares;

public class PagingValidationMiddleware(FieldDelegate next)
{
    private const string SkipArgName = "skip";
    private const string TakeArgName = "take";

    public async Task InvokeAsync(IMiddlewareContext context, INullSafeValidator<PageDto> pageValidator,
        IOptions<BusinessRules> businessRules)
    {
        if (context.Selection.Field.Arguments.Any(x => x.Name is SkipArgName or TakeArgName))
        {
            var skip = context.ArgumentValue<int?>(SkipArgName) ?? 0; // Value by default
            var take = context.ArgumentValue<int?>(TakeArgName) ??
                       businessRules.Value.DefaultPageSize; // Value by default

            var pagination = new PageDto(skip, take);

            if (!pageValidator.IsValid(pagination, out var errors))
                throw GraphQlExceptionHelper.GetException(
                    $"{ErrorMessage.InvalidPagination}: {string.Join(' ', errors)}");
        }

        await next(context);
    }
}