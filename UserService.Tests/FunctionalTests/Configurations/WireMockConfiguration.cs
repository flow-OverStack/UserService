using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using UserService.Tests.Configurations.TestDbContexts;
using UserService.Tests.Extensions;
using UserService.Tests.FunctionalTests.Configurations.Keycloak;
using WireMock;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Types;
using WireMock.Util;

namespace UserService.Tests.FunctionalTests.Configurations;

internal static class WireMockConfiguration
{
    public const string RealmName = "TestRealm";

    private const string FunctionalTestsDirectoryName = "FunctionalTests";
    private const string ResponsesDirectoryName = "TestServerResponses";

    private static WireMockServer? _server;
    public static int Port { get; private set; }

    public static void StartServer(IServiceCollection services)
    {
        SafeStartServer();

        ConfigureWellKnownEndpoints();
        ConfigureTokenEndpoint(services);
        ConfigureUserManagementEndpoints(services);
    }

    private static void ConfigureWellKnownEndpoints()
    {
        _server!.Given(Request.Create().WithPath($"/realms/{RealmName}/.well-known/openid-configuration").UsingGet())
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody(GetMetadata()).WithSuccess());

        _server.Given(Request.Create().WithPath($"/realms/{RealmName}/protocol/openid-connect/certs").UsingGet())
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody(SigningKeyExtensions.GetJwk()).WithSuccess());
    }

    private static void ConfigureTokenEndpoint(IServiceCollection services)
    {
        _server!.Given(Request.Create().WithPath($"/realms/{RealmName}/protocol/openid-connect/token").UsingPost())
            .RespondWith(Response.Create().WithHeader("Content-Type", "application/json")
                .WithCallback(message => HandleTokenRequest(message, services)));
    }

    private static ResponseMessage HandleTokenRequest(IRequestMessage message, IServiceCollection services)
    {
        var body = message.BodyData?.BodyAsFormUrlEncoded;
        if (body == null || !body.TryGetValue("grant_type", out var grantType))
            return BadRequest();

        if (grantType == "password" && !ValidateUserCredentials(body, services))
            return BadRequest();

        return grantType switch
        {
            "password" or "refresh_token" => JsonResponse(new
            {
                access_token = "newAccessToken",
                expires_in = 300,
                refresh_expires_in = 1800,
                refresh_token = "newRefreshToken"
            }),
            "client_credentials" => JsonResponse(new
            {
                access_token = "newAccessToken",
                expires_in = 300,
                refresh_expires_in = 0
            }),
            _ => BadRequest()
        };
    }

    private static bool ValidateUserCredentials(IDictionary<string, string> body, IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<KeycloakDbContext>();

        if (!body.TryGetValue("password", out var reqPassword) ||
            !body.TryGetValue("username", out var username))
            return false;

        var user = dbContext.Set<KeycloakUser>().FirstOrDefault(x => x.Username == username);
        return user?.Password == reqPassword;
    }

    private static void ConfigureUserManagementEndpoints(IServiceCollection services)
    {
        _server!.Given(Request.Create().WithPath($"/admin/realms/{RealmName}/users").UsingPost())
            .RespondWith(Response.Create().WithCallback(message => HandleUserCreation(message, services)));

        _server.Given(Request.Create().WithPath($"/admin/realms/{RealmName}/users").UsingGet().WithParam("username"))
            .RespondWith(Response.Create().WithHeader("Content-Type", "application/json")
                .WithCallback(message => HandleUserSearch(message, services))
                .WithSuccess());

        _server.Given(Request.Create().WithPath($"/admin/realms/{RealmName}/users").UsingPut())
            .RespondWith(Response.Create().WithStatusCode(204));
    }

    private static ResponseMessage HandleUserCreation(IRequestMessage message, IServiceCollection services)
    {
        var user = JsonConvert.DeserializeObject<KeycloakRequestUser>(message.BodyData?.BodyAsString ?? "");
        if (user == null || user.Credentials.All(x => x.Type != "password"))
            return BadRequest();

        using var scope = services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<KeycloakDbContext>();

        dbContext.Set<KeycloakUser>().Add(new KeycloakUser
        {
            Id = Guid.NewGuid(),
            Username = user.Username,
            Password = user.Credentials.First(x => x.Type == "password").Value
        });

        try
        {
            dbContext.SaveChanges();
            return new ResponseMessage { StatusCode = 201 };
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    private static ResponseMessage HandleUserSearch(IRequestMessage message, IServiceCollection services)
    {
        var username = message.GetParameter("username")?.FirstOrDefault();
        if (username == null)
            return BadRequest();

        using var scope = services.BuildServiceProvider().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<KeycloakDbContext>();
        var users = dbContext.Set<KeycloakUser>().Where(x => x.Username.StartsWith(username)).ToArray();

        return JsonResponse(users);
    }

    private static ResponseMessage JsonResponse(object data)
    {
        return new ResponseMessage
        {
            StatusCode = 200,
            BodyData = new BodyData
            {
                BodyAsJson = data,
                DetectedBodyType = BodyType.Json
            }
        };
    }

    private static ResponseMessage BadRequest()
    {
        return new ResponseMessage { StatusCode = 400 };
    }

    private static void SafeStartServer()
    {
        if (_server is { IsStarted: true }) return; //Equals to (_server == null || !_server.IsStarted)
        _server = WireMockServer.Start();
        Port = _server.Ports[0]; //First port
    }


    public static void StopServer()
    {
        _server?.Stop();
    }


    private static string GetMetadata()
    {
        const string metadataFileName = "MetadataResponse.json";

        var response = GetResponse(metadataFileName);

        response = response.Replace("{{Port}}", Port.ToString());
        response = response.Replace("{{Realm}}", RealmName);
        response = response.Replace("{{Issuer}}", SigningKeyExtensions.GetIssuer());

        return response;
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