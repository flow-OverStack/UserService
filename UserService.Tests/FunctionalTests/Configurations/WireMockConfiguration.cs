using System.Reflection;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace UserService.Tests.FunctionalTests.Configurations;

public static class WireMockConfiguration
{
    private const int Port = 5001;
    private const string RealmName = "flowOverStack";

    private const string FunctionalTestsDirectoryName = "FunctionalTests";
    private const string ResponsesDirectoryName = "TestServerResponses";

    private static WireMockServer _server = null!;

    public static WireMockServer StartServer()
    {
        _server = WireMockServer.Start(Port);


        _server.Given(Request.Create().WithPath($"/realms/{RealmName}/.well-known/openid-configuration").UsingGet())
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody(GetMetadata()).WithSuccess());

        return _server;
    }

    public static void StopServer()
    {
        _server.Stop();
    }


    private static string GetMetadata()
    {
        const string metadataFileName = "MetadataResponse.json";
        return GetResponse(metadataFileName);
    }

    private static string GetResponse(string fileName)
    {
        var projectDirectory = Directory.GetParent(AppContext.BaseDirectory); //path to runtime directory

        var currentProjectName =
            string.Join('.',
                Assembly.GetExecutingAssembly().GetName().Name!.Split('.')
                    .Take(2)); //name of current project if naming is of type '<AppName>.<DirectoryName>' (e. g. 'UserService.Api')

        var filePath = GetPath(projectDirectory!, currentProjectName, fileName);

        var response = File.ReadAllText(filePath);

        return response;
    }

    private static string GetPath(DirectoryInfo runtimeDirectory, string currentProjectName, string fileName)
    {
        var currentProjectDirectory = runtimeDirectory;

        while (currentProjectDirectory.Name != currentProjectName)
            currentProjectDirectory =
                currentProjectDirectory.Parent!; //getting path to current project directory from runtime directory


        var filePath = Path.Combine(currentProjectDirectory.FullName, FunctionalTestsDirectoryName,
            ResponsesDirectoryName,
            fileName);

        return filePath;
    }
}