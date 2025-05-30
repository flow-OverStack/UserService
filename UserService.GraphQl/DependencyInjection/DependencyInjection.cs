using GraphQL.Server.Ui.Voyager;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using UserService.Domain.Settings;
using UserService.GraphQl.DataLoaders;
using UserService.GraphQl.ErrorFilters;
using UserService.GraphQl.Middlewares;
using UserService.GraphQl.Types;

namespace UserService.GraphQl.DependencyInjection;

public static class DependencyInjection
{
    private const string GraphQlEndpoint = "/graphql";
    private const string GraphQlVoyagerEndpoint = "/graphql-voyager";

    /// <summary>
    ///     Adds GraphQl services
    /// </summary>
    /// <param name="services"></param>
    public static void AddGraphQl(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .AddQueryType<Queries>()
            .AddType<UserType>()
            .AddType<RoleType>()
            .AddSorting()
            .AddFiltering()
            .AddErrorFilter<PublicErrorFilter>()
            .AddDataLoader<UserDataLoader>()
            .AddDataLoader<RoleDataLoader>()
            .AddDataLoader<GroupUserDataLoader>()
            .AddDataLoader<GroupRoleDataLoader>()
            .AddApolloFederation()
            .ModifyPagingOptions(opt =>
            {
                using var provider = services.BuildServiceProvider();
                using var scope = provider.CreateScope();
                var defaultSize = scope.ServiceProvider.GetRequiredService<IOptions<BusinessRules>>().Value
                    .DefaultPageSize;

                opt.DefaultPageSize = defaultSize;
                opt.IncludeTotalCount = true;
            })
            .UseField<PagingValidationMiddleware>()
            .ModifyCostOptions(opt => opt.MaxFieldCost *= 2);
    }

    /// <summary>
    ///     Enables the use of GraphQl services
    /// </summary>
    /// <param name="app"></param>
    public static void UseGraphQl(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseGraphQLVoyager(GraphQlVoyagerEndpoint, new VoyagerOptions { GraphQLEndPoint = GraphQlEndpoint });

        app.MapGraphQL();
    }
}