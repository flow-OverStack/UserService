using UserService.Domain.Dtos.Request.Grpahql;
using UserService.Domain.Entities;

namespace UserService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GraphQlGetAllResponse
{
    public GraphQlGetAllData Data { get; set; }
}

internal class GraphQlGetAllData
{
    public PaginatedResult<User> Users { get; set; }
    public PaginatedResult<Role> Roles { get; set; }
}