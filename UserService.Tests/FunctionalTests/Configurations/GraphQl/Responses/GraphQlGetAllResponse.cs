using UserService.Domain.Entities;
using UserService.Tests.FunctionalTests.Configurations.GraphQl.Entities;

namespace UserService.Tests.FunctionalTests.Configurations.GraphQl.Responses;

internal class GraphQlGetAllResponse
{
    public GraphQlGetAllData Data { get; set; }
}

internal class GraphQlGetAllData
{
    public GraphQlOffsetPaginatedResponse<GraphQlUser> Users { get; set; }
    public GraphQlOffsetPaginatedResponse<Role> Roles { get; set; }
    public GraphQlCursorPaginatedResponse<ReputationRecord> ReputationRecords { get; set; }
    public GraphQlOffsetPaginatedResponse<ReputationRule> ReputationRules { get; set; }
}