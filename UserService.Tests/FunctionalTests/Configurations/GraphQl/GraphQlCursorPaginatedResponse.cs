namespace UserService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GraphQlCursorPaginatedResponse<T>
{
    public List<GraphQlEdge<T>> Edges { get; set; }
    public int TotalCount { get; set; }
}

internal class GraphQlEdge<T>
{
    public string Cursor { get; set; }
    public T Node { get; set; }
}