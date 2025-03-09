using UserService.Domain.Entity;

namespace UserService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GraphQlGetUserByIdResponse
{
    public GraphQlGetUserByIdData Data { get; set; }
}

internal class GraphQlGetUserByIdData
{
    public User User { get; set; }
}