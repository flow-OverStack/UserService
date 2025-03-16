using System.Net;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using UserService.Tests.Constants;
using UserService.Tests.FunctionalTests.Configurations.Keycloak;
using UserService.Tests.FunctionalTests.Configurations.Keycloak.HttpModels;
using UserService.Tests.FunctionalTests.Helpers;
using WireMock;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Types;
using WireMock.Util;

namespace UserService.Tests.FunctionalTests.Extensions;

internal static class WireMockIdentityServerExtensions
{
    public const string RealmName = "TestRealm";

    private const string FunctionalTestsDirectoryName = "FunctionalTests";
    private const string ConfigurationsDirectoryName = "Configurations";
    private const string ResponsesDirectoryName = "TestServerResponses";

    public static WireMockServer StartServer(this WireMockServer server, IServiceCollection services)
    {
        server = server.SafeStartServer();

        server.ConfigureWellKnownEndpoints();
        server.ConfigureTokenEndpoint(services);
        server.ConfigureUserManagementEndpoints(services);

        return server;
    }

    private static void ConfigureWellKnownEndpoints(this WireMockServer server)
    {
        server.Given(Request.Create().WithPath($"/realms/{RealmName}/.well-known/openid-configuration").UsingGet())
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody(GetMetadata(server.Port)).WithSuccess());

        server.Given(Request.Create().WithPath($"/realms/{RealmName}/protocol/openid-connect/certs").UsingGet())
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody(TokenHelper.GetJwk()).WithSuccess());
    }

    private static void ConfigureTokenEndpoint(this WireMockServer server, IServiceCollection services)
    {
        server.Given(Request.Create().WithPath($"/realms/{RealmName}/protocol/openid-connect/token").UsingPost())
            .RespondWith(Response.Create().WithHeader("Content-Type", "application/json")
                .WithCallback(message => HandleTokenRequest(message, services)));
    }

    private static ResponseMessage HandleTokenRequest(IRequestMessage message, IServiceCollection services)
    {
        var body = message.BodyData?.BodyAsFormUrlEncoded;
        if (body == null || !body.TryGetValue("grant_type", out var grantType))
            return BadRequest();

        if (grantType == "password" && !ValidateUserCredentials(body, services))
            return new ResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                BodyData = new BodyData
                {
                    BodyAsString = """
                                   {
                                       "error": "invalid_grant",
                                       "error_description": "Invalid user credentials"
                                   }
                                   """,
                    DetectedBodyType = BodyType.String
                }
            };

        if (grantType == "refresh_token" && body.TryGetValue("refresh_token", out var refreshToken) &&
            refreshToken == TestConstants.WrongRefreshToken)
            return new ResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                BodyData = new BodyData
                {
                    BodyAsString = """
                                   {
                                       "error": "invalid_grant",
                                       "error_description": "Invalid refresh token"
                                   }
                                   """,
                    DetectedBodyType = BodyType.String
                }
            };

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

        return body.TryGetValue("username", out var username) &&
               body.TryGetValue("password", out var reqPassword) &&
               dbContext.Set<KeycloakUser>().Any(x => x.Username == username && x.Password == reqPassword);
    }

    private static void ConfigureUserManagementEndpoints(this WireMockServer server, IServiceCollection services)
    {
        server.Given(Request.Create().WithPath($"/admin/realms/{RealmName}/users").UsingPost())
            .RespondWith(Response.Create().WithCallback(message => HandleUserCreation(message, services)));

        server.Given(Request.Create().WithPath($"/admin/realms/{RealmName}/users").UsingGet().WithParam("username"))
            .RespondWith(Response.Create().WithHeader("Content-Type", "application/json")
                .WithCallback(message => HandleUserSearch(message, services))
                .WithSuccess());

        server.Given(Request.Create().WithPath($"/admin/realms/{RealmName}/users/*").UsingPut())
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.NoContent));
    }

    private static ResponseMessage HandleUserCreation(IRequestMessage message, IServiceCollection services)
    {
        var user = JsonConvert.DeserializeObject<KeycloakRequestUser>(message.BodyData?.BodyAsString ?? string.Empty);
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
            return new ResponseMessage { StatusCode = HttpStatusCode.Created };
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
            StatusCode = HttpStatusCode.OK,
            BodyData = new BodyData
            {
                BodyAsJson = data,
                DetectedBodyType = BodyType.Json
            }
        };
    }

    private static ResponseMessage BadRequest()
    {
        return new ResponseMessage { StatusCode = HttpStatusCode.BadRequest };
    }

    private static WireMockServer SafeStartServer(this WireMockServer server)
    {
        if (server is { IsStarted: true }) return server; //Equals to (_server == null || !_server.IsStarted)
        server = WireMockServer.Start();
        return server;
    }


    public static void StopServer(this WireMockServer server)
    {
        server.Stop();
    }


    private static string GetMetadata(int serverPort)
    {
        const string metadataFileName = "MetadataResponse.json";

        var response = GetResponse(metadataFileName);

        response = response.Replace("{{Port}}", serverPort.ToString());
        response = response.Replace("{{Realm}}", RealmName);
        response = response.Replace("{{Issuer}}", TokenHelper.GetIssuer());

        return response;
    }

    private static string GetResponse(string fileName)
    {
        var projectDirectory = Directory.GetParent(AppContext.BaseDirectory); //path to runtime directory

        var currentProjectName =
            string.Join('.',
                Assembly.GetExecutingAssembly().GetName().Name!.Split('.')
                    .Take(2)); //name of current project if naming is of type '<AppName>.<DirectoryName>' (e. g. 'QuestionService.Api')

        var filePath = GetPathToConfiguration(projectDirectory!, currentProjectName, fileName, ResponsesDirectoryName);

        var response = File.ReadAllText(filePath);

        return response;
    }

    private static string GetPathToConfiguration(DirectoryInfo runtimeDirectory, string currentProjectName,
        string fileName, params string[] configurationSubdirectories)
    {
        var currentProjectDirectory = runtimeDirectory;

        while (currentProjectDirectory.Name != currentProjectName)
            currentProjectDirectory =
                currentProjectDirectory.Parent!; //getting path to current project directory from runtime directory


        var configurationSubdirectoriesPath = Path.Combine(configurationSubdirectories);
        var filePath = Path.Combine(currentProjectDirectory.FullName, FunctionalTestsDirectoryName,
            ConfigurationsDirectoryName, configurationSubdirectoriesPath, fileName);

        return filePath;
    }
}