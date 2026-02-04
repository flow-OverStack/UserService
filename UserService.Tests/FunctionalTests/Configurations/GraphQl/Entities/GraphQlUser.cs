using UserService.Domain.Entities;

namespace UserService.Tests.FunctionalTests.Configurations.GraphQl.Entities;

internal class GraphQlUser : User
{
    public int CurrentReputation { get; set; }
    public int RemainingReputation { get; set; }
}