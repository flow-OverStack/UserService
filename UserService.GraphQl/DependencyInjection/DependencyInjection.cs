using GraphQL.Server.Ui.Voyager;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserService.Domain.Entities;
using UserService.GraphQl.DataLoaders;
using UserService.GraphQl.ErrorFilters;
using UserService.GraphQl.Types;
using UserService.GraphQl.Types.Pagination;

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
            .AddType<PageInfoType>()
            .AddType<PaginatedResultType<User>>()
            .AddType<PaginatedResultType<Role>>()
            .AddSorting()
            .AddFiltering()
            .AddErrorFilter<PublicErrorFilter>()
            .AddDataLoader<UserDataLoader>()
            .AddDataLoader<RoleDataLoader>()
            .AddDataLoader<GroupUserDataLoader>()
            .AddDataLoader<GroupRoleDataLoader>()
            .AddApolloFederation();
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