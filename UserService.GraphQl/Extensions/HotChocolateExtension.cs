using HotChocolate.Language;
using UserService.Domain.Dtos.Pagination;
using UserService.Domain.Enums;

namespace UserService.GraphQl.Extensions;

public static class HotChocolateExtension
{
    public static IEnumerable<SortOrder> ToSortOrder(this ListValueNode? listValueNode)
    {
        if (listValueNode == null) return [];

        return
        [
            .. listValueNode.Items
                .Select(item => item as ObjectValueNode
                                ?? throw new ArgumentException($"Item must be of type {nameof(ObjectValueNode)}."))
                .SelectMany(objectValueNode => objectValueNode.Fields.Select(ParseOrderFromField))
        ];
    }

    private static SortOrder ParseOrderFromField(ObjectFieldNode field)
    {
        var columnName = field.Name.Value;

        if (field.Value is not EnumValueNode directionEnumNode)
            throw new ArgumentException($"The direction value for field '{columnName}' must be an enum (ASC or DESC).");

        var directionString = directionEnumNode.Value;

        if (!Enum.TryParse<SortDirection>(directionString, true, out var direction))
            throw new ArgumentException($"Invalid sort direction '{directionString}' for field '{columnName}'.");

        return new SortOrder(columnName, direction);
    }
}