using UserService.Domain.Entities;

namespace UserService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GraphQlGetAllResponse
{
    public GraphQlGetAllData Data { get; set; }
}

internal class GraphQlGetAllData
{
    public GraphQlPaginatedResponse<User> Users { get; set; }
    public GraphQlPaginatedResponse<Role> Roles { get; set; }
}