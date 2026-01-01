using UserService.Domain.Entities;

namespace UserService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GetAllByIdsResponse
{
    public GetAllByIdsData Data { get; set; }
}

internal class GetAllByIdsData
{
    public User? User { get; set; }
    public Role? Role { get; set; }
    public ReputationRecord? ReputationRecord { get; set; }
    public ReputationRule? ReputationRule { get; set; }
}