using UserService.Domain.Entities;

namespace UserService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GraphQlGetUserByIdResponse
{
    public GraphQlGetUserByIdData Data { get; set; }
}

internal class GraphQlGetUserByIdData
{
    public User? User { get; set; }
}