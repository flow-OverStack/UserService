using UserService.Tests.FunctionalTests.Configurations.GraphQl.Entities;

namespace UserService.Tests.FunctionalTests.Configurations.GraphQl.Responses;

internal class GraphQlGetMeResponse
{
    public GraphQlGetMeData Data { get; set; }
}

internal class GraphQlGetMeData
{
    public GraphQlUser? Me { get; set; }
}