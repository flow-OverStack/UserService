using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UserService.Api.Filters;

/// <inheritdoc />
public class ObsoleteDescriptionOperationFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var obsolete = context.MethodInfo.GetCustomAttribute<ObsoleteAttribute>()
                       ?? GetObsoleteFromHierarchy(context.MethodInfo.DeclaringType);

        if (obsolete == null) return;

        operation.Deprecated = true;

        if (!string.IsNullOrWhiteSpace(obsolete.Message))
            operation.Description = $"Deprecated: {obsolete.Message}\n{operation.Description}";
    }

    /// <summary>
    ///     Searches for the <see cref="ObsoleteAttribute" /> in the specified type or its hierarchy, returning the first match
    ///     found.
    /// </summary>
    /// <param name="type">
    ///     The type to search for the <see cref="ObsoleteAttribute" />. The search will also include the base
    ///     types up to but not including <see cref="object" />.
    /// </param>
    /// <returns>The <see cref="ObsoleteAttribute" /> if found in the type hierarchy; otherwise, null.</returns>
    private static ObsoleteAttribute? GetObsoleteFromHierarchy(Type? type)
    {
        while (type != null && type != typeof(object))
        {
            var attr = type.GetCustomAttribute<ObsoleteAttribute>();
            if (attr != null)
                return attr;

            type = type.BaseType;
        }

        return null;
    }
}