namespace UserService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GraphQlPaginatedResponse<T>
{
    public List<T> Items { get; set; }

    public int TotalCount { get; set; }
}