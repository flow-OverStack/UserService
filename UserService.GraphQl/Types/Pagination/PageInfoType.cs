using UserService.Domain.Dtos.Request.Grpahql;

namespace UserService.GraphQl.Types.Pagination;

public class PageInfoType : ObjectType<PageInfo>
{
    protected override void Configure(IObjectTypeDescriptor<PageInfo> descriptor)
    {
        descriptor.Name($"Offset{nameof(PageInfo)}");
        descriptor.Description("Pagination metadata");

        descriptor.Field(x => x.Page).Description("Current page number");
        descriptor.Field(x => x.Size).Description("Number of items per page");
        descriptor.Field(x => x.TotalPages).Description("Total number of pages");
        descriptor.Field(x => x.TotalItems).Description("Total number of items");
    }
}