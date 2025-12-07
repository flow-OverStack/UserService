using GraphQL.Server.Ui.Voyager;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using UserService.Application.Settings;
using UserService.GraphQl.DataLoaders;
using UserService.GraphQl.ErrorFilters;
using UserService.GraphQl.Types;
using UserService.GraphQl.Types.Sharable;

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
            .AddType<ReputationRecordType>()
            .AddType<ReputationRuleType>()
            .AddTypeExtension<CollectionSegmentInfoType>()
            .AddSorting()
            .AddFiltering()
            .AddErrorFilter<PublicErrorFilter>()
            .AddDataLoader<UserDataLoader>()
            .AddDataLoader<RoleDataLoader>()
            .AddDataLoader<ReputationRuleDataLoader>()
            .AddDataLoader<ReputationRecordDataLoader>()
            .AddDataLoader<CurrentReputationDataLoader>()
            .AddDataLoader<RemainingReputationDataLoader>()
            .AddDataLoader<GroupRoleUserDataLoader>()
            .AddDataLoader<GroupRoleDataLoader>()
            .AddDataLoader<GroupReputationRecordDataLoader>()
            .AddDataLoader<GroupReputationRuleReputationRecordDataLoader>()
            .AddApolloFederation(FederationVersion.Federation23)
            .ModifyPagingOptions(opt =>
            {
                using var provider = services.BuildServiceProvider();
                using var scope = provider.CreateAsyncScope();
                var defaultSize = scope.ServiceProvider.GetRequiredService<IOptions<PaginationRules>>().Value
                    .DefaultPageSize;

                opt.DefaultPageSize = defaultSize;
                opt.IncludeTotalCount = true;
            })
            .AddDbContextCursorPagingProvider()
            .ModifyCostOptions(opt => opt.MaxFieldCost *= 3);
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