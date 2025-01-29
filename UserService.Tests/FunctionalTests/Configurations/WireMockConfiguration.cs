using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using UserService.Tests.Configurations.TestDbContexts;
using UserService.Tests.Extensions;
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
        _server?.Stop();
        _server = WireMockServer.Start();
        Port = _server.Ports[0]; //First port


        _server.Given(Request.Create().WithPath($"/realms/{RealmName}/.well-known/openid-configuration").UsingGet())
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody(GetMetadata()).WithSuccess());

        _server.Given(Request.Create().WithPath($"/realms/{RealmName}/protocol/openid-connect/certs").UsingGet())
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody(SigningKeyExtensions.GetJwk()).WithSuccess());

        _server.Given(Request.Create().WithPath($"/realms/{RealmName}/protocol/openid-connect/token").UsingPost())
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithCallback(message =>
                {
                    var body = message.BodyData!.BodyAsFormUrlEncoded;
                    if (body == null)
                        return new ResponseMessage
                        {
                            StatusCode = 400
                        };

                    if (!body.TryGetValue("grant_type", out var grantType))
                        return new ResponseMessage
                        {
                            StatusCode = 400
                        };

                    return grantType switch
                    {
                        "password" => new ResponseMessage
                        {
                            StatusCode = 200,
                            BodyData = new BodyData
                            {
                                BodyAsString = """
                                               {
                                                   "access_token": "newAccessToken",
                                                   "expires_in": 300,
                                                   "refresh_expires_in": 1800,
                                                   "refresh_token": "newRefreshToken"
                                               }
                                               """,
                                DetectedBodyType = BodyType.String
                            }
                        },
                        "client_credentials" => new ResponseMessage
                        {
                            StatusCode = 200,
                            BodyData = new BodyData
                            {
                                BodyAsString = """
                                               {
                                                   "access_token": "newAccessToken",
                                                   "expires_in": 300,
                                                   "refresh_expires_in": 0,
                                               }
                                               """,
                                DetectedBodyType = BodyType.String
                            }
                        },
                        "refresh_token" => new ResponseMessage
                        {
                            StatusCode = 200,
                            BodyData = new BodyData
                            {
                                BodyAsString = """
                                               {
                                                   "access_token": "newAccessToken",
                                                   "expires_in": 300,
                                                   "refresh_expires_in": 1800,
                                                   "refresh_token": "newRefreshToken"
                                               }
                                               """,
                                DetectedBodyType = BodyType.String
                            }
                        },
                        _ => new ResponseMessage { StatusCode = 400 }
                    };
                }));

        _server.Given(Request.Create().WithPath($"/admin/realms/{RealmName}/users").UsingPost())
            .RespondWith(Response.Create().WithCallback(message =>
            {
                var body = message.BodyData?.BodyAsString;
                var user = JsonConvert.DeserializeObject<KeycloakUser>(body!);
                if (user == null)
                    return new ResponseMessage { StatusCode = 400 };

                using var scope = services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<KeycloakDbContext>();

                user.Id = Guid.NewGuid();
                dbContext.Set<KeycloakUser>().Add(user);
                dbContext.SaveChanges();

                return new ResponseMessage { StatusCode = 201 };
            }));

        const string usernameParam = "username";
        _server.Given(Request.Create().WithPath($"/admin/realms/{RealmName}/users").UsingGet().WithParam(usernameParam))
            .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithCallback(message =>
                {
                    var username = message.GetParameter(usernameParam)?.FirstOrDefault();
                    if (username == null)
                        return new ResponseMessage
                        {
                            StatusCode = 400
                        };

                    using var scope = services.BuildServiceProvider().CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<KeycloakDbContext>();
                    var users = dbContext.Set<KeycloakUser>().AsQueryable().Where(x => x.Username.StartsWith(username))
                        .ToArray();

                    return new ResponseMessage
                    {
                        StatusCode = 200,
                        BodyData = new BodyData
                        {
                            BodyAsJson = users,
                            DetectedBodyType = BodyType.Json
                        }
                    };
                })
                .WithSuccess());

        _server.Given(Request.Create().WithPath($"/admin/realms/{RealmName}/users").UsingPut())
            .RespondWith(Response.Create()
                .WithStatusCode(204));
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