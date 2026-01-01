namespace UserService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GraphQlOffsetPaginatedResponse<T>
{
    public List<T> Items { get; set; }

    public int TotalCount { get; set; }
}