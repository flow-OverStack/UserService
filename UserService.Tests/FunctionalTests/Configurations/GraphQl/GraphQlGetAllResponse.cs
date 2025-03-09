using UserService.Domain.Entity;

namespace UserService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GraphQlGetAllResponse
{
    public GraphQlGetAllData Data { get; set; }
}

internal class GraphQlGetAllData
{
    public List<User> Users { get; set; }
    public List<Role> Roles { get; set; }
}