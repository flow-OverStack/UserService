namespace UserService.Tests.FunctionalTests.Configurations.GrpahQl;

internal class GraphQlErrorResponse
{
    public List<GraphQlError> Errors { get; set; }
}

internal class GraphQlError
{
    public GraphQlErrorExtensions Extensions { get; set; }
}

internal class GraphQlErrorExtensions
{
    public string Code { get; set; }
}