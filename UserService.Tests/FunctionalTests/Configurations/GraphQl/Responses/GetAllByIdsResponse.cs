using UserService.Domain.Entities;
using UserService.Tests.FunctionalTests.Configurations.GraphQl.Entities;

namespace UserService.Tests.FunctionalTests.Configurations.GraphQl.Responses;

internal class GetAllByIdsResponse
{
    public GetAllByIdsData Data { get; set; }
}

internal class GetAllByIdsData
{
    public GraphQlUser? User { get; set; }
    public Role? Role { get; set; }
    public ReputationRecord? ReputationRecord { get; set; }
    public ReputationRule? ReputationRule { get; set; }
}