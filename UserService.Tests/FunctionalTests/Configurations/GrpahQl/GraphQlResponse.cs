using UserService.Domain.Entity;

namespace UserService.Tests.FunctionalTests.Configurations.GrpahQl;

internal class GraphQlResponse
{
    public GraphQlData Data { get; set; }
}

internal class GraphQlData
{
    public List<User> Users { get; set; }
    public List<Role> Roles { get; set; }
}