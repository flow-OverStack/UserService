using UserService.Domain.Entities;

namespace UserService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GraphQlGetAllResponse
{
    public GraphQlGetAllData Data { get; set; }
}

internal class GraphQlGetAllData
{
    public GraphQlOffsetPaginatedResponse<User> Users { get; set; }
    public GraphQlOffsetPaginatedResponse<Role> Roles { get; set; }
    public GraphQlCursorPaginatedResponse<ReputationRecord> ReputationRecords { get; set; }
    public GraphQlOffsetPaginatedResponse<ReputationRule> ReputationRules { get; set; }
}