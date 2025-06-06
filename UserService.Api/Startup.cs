using System.Reflection;
using Asp.Versioning;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using UserService.Domain.Settings;
using Path = System.IO.Path;

namespace UserService.Api;

/// <summary>
///     A utility class that provides extension methods for configuring and setting up the application's services and middleware.
/// </summary>
public static class Startup
{
    private const string AppStartupSectionName = "AppStartupSettings";
    private const string AppPortsSectionName = "Ports";
    private const string OpenTelemetrySectionName = "OpenTelemetrySettings";
    private const string AspireDashboardSectionName = "AspireDashboardUrl";
    private const string AppStartupUrlLogName = "AppStartupUrlLog";
    private const string GrpcPortName = "GrpcPort";
    private const string RestApiPortName = "RestApiPort";
    private const string UseHttpsForRestApiName = "UseHttpsForRestApi";
    private const string ServiceName = "UserService";

    /// <summary>
    ///     Configures JWT Bearer authentication and authorization services for the application.
    /// </summary>
    /// <param name="services">The service collection to which authentication and authorization services are added.</param>
    public static void AddAuthenticationAndAuthorization(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var keycloakSettings =
                services.BuildServiceProvider().GetRequiredService<IOptions<KeycloakSettings>>().Value;

            options.RequireHttpsMetadata = false;
            options.MetadataAddress = keycloakSettings.MetadataAddress;
            options.Audience = keycloakSettings.Audience;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
    }

    /// <summary>
    ///     Configures and adds Swagger documentation generation with JWT authentication support to the service collection.
    ///     Includes API versioning, security definitions, and XML documentation.
    /// </summary>
    /// <param name="services">The service collection to which Swagger services are added.</param>
    public static void AddSwagger(this IServiceCollection services)
    {
        const string apiVersion = "v1";

        services.AddApiVersioning()
            .AddApiExplorer(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
            });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(apiVersion, new OpenApiInfo
            {
                Version = "v1",
                Title = "UserService.Api",
                Description = "UserService api v1",
                //maybe add in future
                //TermsOfService = termsOfServiceUrl,
                Contact = new OpenApiContact
                {
                    Name = "UserService api contact"
                    //maybe add in future
                    //Url = termsOfServiceUrl
                }
            });

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please write valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header
                    },
                    []
                }
            });

            var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
        });
    }


    /// <summary>Logs all URLs on which the application is listening when it starts.</summary>
    /// <param name="app">The web application to which the middleware is added.</param>
    public static void LogListeningUrls(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var hosts = app.GetHosts().ToList();

            var appStartupHostLog =
                app.Configuration.GetSection(AppStartupSectionName).GetValue<string>(AppStartupUrlLogName);

            hosts.ForEach(host => Log.Information("{0}{1}", appStartupHostLog, host));
        });
    }

    /// <summary>
    ///     Configures Kestrel server to listen on specific ports for gRPC and REST API endpoints.
    ///     Reads port configuration from application settings and sets up appropriate protocols and HTTPS settings.
    /// </summary>
    /// <param name="hostBuilder">The web host builder to configure.</param>
    /// <param name="configuration">The configuration containing port settings.</param>
    public static void ConfigurePorts(this IWebHostBuilder hostBuilder, IConfiguration configuration)
    {
        var grpcPort = configuration.GetSection(AppStartupSectionName).GetSection(AppPortsSectionName)
            .GetValue<int>(GrpcPortName);

        var apiPort = configuration.GetSection(AppStartupSectionName).GetSection(AppPortsSectionName)
            .GetValue<int>(RestApiPortName);

        var useHttpsForApi = configuration.GetSection(AppStartupSectionName).GetSection(AppPortsSectionName)
            .GetValue<bool>(UseHttpsForRestApiName);

        hostBuilder.ConfigureKestrel(opt =>
        {
            opt.ListenAnyIP(grpcPort, listenOpt => listenOpt.Protocols = HttpProtocols.Http2);
            opt.ListenAnyIP(apiPort, listenOpt =>
            {
                listenOpt.Protocols = HttpProtocols.Http1AndHttp2;
                if (useHttpsForApi) listenOpt.UseHttps();
            });
        });
    }

    /// <summary>
    ///     Configures Hangfire with PostgreSQL storage and adds it to the service collection.
    ///     Sets up job retry policies, serialization settings, and logging integration.
    /// </summary>
    /// <param name="services">The service collection to which Hangfire services are added.</param>
    /// <param name="configuration">The configuration containing database connection settings.</param>
    public static void AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        const string postgresSqlConnectionName = "PostgresSQL";

        services.AddHangfire(x => x.UsePostgreSqlStorage(options =>
            {
                var connectionString = configuration.GetConnectionString(postgresSqlConnectionName);
                options.UseNpgsqlConnection(connectionString);
            })
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSerilogLogProvider()
            .UseFilter(new AutomaticRetryAttribute
            {
                Attempts = 10,
                DelaysInSeconds = [30, 60, 300, 600, 1800, 43200, 86400] //30sec, 1min, 5min, 10min, 1h, 12h, 24h
            }));

        services.AddHangfireServer();
    }

    /// <summary>
    ///     Enables the Hangfire dashboard in development environments.
    ///     This provides a web interface for monitoring and managing background jobs.
    /// </summary>
    /// <param name="app">The web application to which Hangfire middleware is added.</param>
    public static void UseHangfire(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseHangfireDashboard();
    }

    /// <summary>
    ///     Configures and adds OpenTelemetry for observability, including metrics, tracing, and logging instrumentation.
    /// </summary>
    /// <param name="builder">The instance of <see cref="WebApplicationBuilder" /> being configured.</param>
    public static void AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        var openTelemetryConfiguration =
            builder.Configuration.GetSection(AppStartupSectionName).GetSection(OpenTelemetrySectionName);
        //const string jaegerUrlName = "JaegerUrl";

        var aspireDashboardUri = new Uri(openTelemetryConfiguration.GetValue<string>(AspireDashboardSectionName)!);
        //var jaegerUri = new Uri(openTelemetryConfiguration.GetValue<string>(jaegerUrlName));

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(res => res.AddService(ServiceName))
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                metrics.AddOtlpExporter(options => options.Endpoint = aspireDashboardUri);
            })
            .WithTracing(traces =>
            {
                traces.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();

                traces.AddOtlpExporter(options => options.Endpoint = aspireDashboardUri);
                //.AddOtlpExporter(options => options.Endpoint = jaegerUri);
            });

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.AddOtlpExporter(options => options.Endpoint = aspireDashboardUri);
            logging.IncludeScopes = true;
            logging.IncludeFormattedMessage = true;
        });
    }

    /// <summary>
    ///     Configures structured logging for the application using Serilog with OpenTelemetry integration.
    /// </summary>
    /// <param name="host">The host builder used to configure the application.</param>
    /// <param name="appConfiguration">The application configuration from which logging settings are retrieved.</param>
    public static void AddLogging(this IHostBuilder host, IConfiguration appConfiguration)
    {
        const string serviceNameKey = "service.name";
        const string serviceInstanceIdKey = "service.instance.id";

        host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration).WriteTo
            .OpenTelemetry(options =>
            {
                options.Endpoint = appConfiguration.GetSection(AppStartupSectionName)
                    .GetSection(OpenTelemetrySectionName).GetValue<string>(AspireDashboardSectionName);
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    [serviceNameKey] = ServiceName,
                    [serviceInstanceIdKey] = Guid.NewGuid().ToString()
                };
            }));
    }

    private static IEnumerable<string> GetHosts(this WebApplication app)
    {
        HashSet<string> hosts = [];

        var serverAddressesFeature = ((IApplicationBuilder)app).ServerFeatures.Get<IServerAddressesFeature>();
        serverAddressesFeature?.Addresses.ToList().ForEach(x => hosts.Add(x));

        return hosts;
    }
}