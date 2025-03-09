namespace UserService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GraphQlErrorResponse
{
    public List<GraphQlError> Errors { get; set; }
}

internal class GraphQlError
{
    public string Message { get; set; }
    public GraphQlErrorExtensions Extensions { get; set; }
}

internal class GraphQlErrorExtensions
{
    public string Code { get; set; }
}