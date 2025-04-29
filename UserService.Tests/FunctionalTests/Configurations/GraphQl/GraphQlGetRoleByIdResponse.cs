using UserService.Domain.Entities;

namespace UserService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GraphQlGetRoleByIdResponse
{
    public GraphQlGetRoleByIdData Data { get; set; }
}

internal class GraphQlGetRoleByIdData
{
    public Role? Role { get; set; }
}