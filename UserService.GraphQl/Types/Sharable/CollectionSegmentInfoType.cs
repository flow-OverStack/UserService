using HotChocolate.ApolloFederation.Types;
using HotChocolate.Types.Pagination;

namespace UserService.GraphQl.Types.Sharable;

public class CollectionSegmentInfoType : ObjectTypeExtension<CollectionSegmentInfo>
{
    protected override void Configure(IObjectTypeDescriptor<CollectionSegmentInfo> descriptor)
    {
        descriptor.Field(x => x.HasNextPage).Shareable();
        descriptor.Field(x => x.HasPreviousPage).Shareable();
    }
}