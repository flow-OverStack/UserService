using UserService.Domain.Dtos.Request.Grpahql;

namespace UserService.GraphQl.Types.Pagination;

public class PaginatedResultType<T> : ObjectType<PaginatedResult<T>>
    where T : class
{
    protected override void Configure(IObjectTypeDescriptor<PaginatedResult<T>> descriptor)
    {
        descriptor.Name($"Paginated{typeof(T).Name}Result");
        descriptor.Description($"A paginated list of {typeof(T).Name} items with pagination info.");

        descriptor.Field(x => x.PageInfo)
            .Description("Information about pagination state.");

        descriptor.Field(x => x.Items)
            .Description($"The list of {typeof(T).Name} items on the current page.");
    }
}