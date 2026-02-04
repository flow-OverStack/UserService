using UserService.Tests.FunctionalTests.Configurations.GraphQl.Entities;

namespace UserService.Tests.FunctionalTests.Configurations.GraphQl.Responses;

internal class GraphQlGetUserByIdResponse
{
    public GraphQlGetUserByIdData Data { get; set; }
}

internal class GraphQlGetUserByIdData
{
    public GraphQlUser? User { get; set; }
}