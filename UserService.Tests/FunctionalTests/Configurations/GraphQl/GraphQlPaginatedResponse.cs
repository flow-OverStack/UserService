namespace UserService.Tests.FunctionalTests.Configurations.GraphQl;

public class GraphQlPaginatedResponse<T>
{
    public List<T> Items { get; set; }

    public int TotalCount { get; set; }
}