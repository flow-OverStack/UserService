using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security;
using Asp.Versioning;
using Confluent.Kafka;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using UserService.Api.Filters;
using UserService.Cache.Settings;
using UserService.DAL;
using UserService.Keycloak.Settings;
using UserService.Messaging.Settings;
using Path = System.IO.Path;

namespace UserService.Api;

/// <summary>
///     A utility class that provides extension methods for configuring and setting up the application's services and middleware.
/// </summary>
public static class Startup
{
    private const string AppStartupSectionName = "AppStartupSettings";
    private const string AppPortsSectionName = "Ports";
    private const string TelemetrySectionName = "TelemetrySettings";
    private const string AspireDashboardUrlName = "AspireDashboardUrl";
    private const string ElasticSearchUrlName = "ElasticSearchUrl";
    private const string JaegerUrlName = "JaegerUrl";
    private const string LogstashUrlName = "LogstashUrl";
    private const string PrometheusUrlName = "PrometheusUrl";
    private const string AspireDashboardHealthCheckUrlName = "AspireDashboardHealthCheckUrl";
    private const string JaegerHealthCheckUrlName = "JaegerHealthCheckUrl";
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

            // Maintains original OAuth2 claims for reliable microservice communication.
            options.MapInboundClaims = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                NameClaimType = JwtRegisteredClaimNames.PreferredUsername
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
            options.OperationFilter<ObsoleteDescriptionOperationFilter>();
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
                //5sec, 10 sec, 15sec, 30sec, 1min, 5min, 10min, 1h, 12h, 24h
                DelaysInSeconds = [5, 10, 15, 30, 60, 300, 600, 1800, 43200, 86400]
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
        var telemetrySection =
            builder.Configuration.GetSection(AppStartupSectionName).GetSection(TelemetrySectionName);

        var aspireDashboardUri = new Uri(telemetrySection.GetValue<string>(AspireDashboardUrlName)!);
        var jaegerUri = new Uri(telemetrySection.GetValue<string>(JaegerUrlName)!);

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(res => res.AddService(ServiceName))
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddMeter(InstrumentationOptions.MeterName)
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                metrics.AddOtlpExporter(options => options.Endpoint = aspireDashboardUri).AddPrometheusExporter();
            })
            .WithTracing(traces =>
            {
                traces.AddAspNetCoreInstrumentation()
                    .AddSource(DiagnosticHeaders.DefaultListenerName)
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();

                traces.AddOtlpExporter(options => options.Endpoint = aspireDashboardUri)
                    .AddOtlpExporter(options => options.Endpoint = jaegerUri);
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
                    .GetSection(TelemetrySectionName).GetValue<string>(AspireDashboardUrlName);
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    [serviceNameKey] = ServiceName,
                    [serviceInstanceIdKey] = Guid.NewGuid()
                };
            }));
    }

    /// <summary>
    ///     Adds health checks to the application, including checks for database, Kafka, Elasticsearch, and Hangfire services.
    /// </summary>
    /// <param name="services">The service collection to which health check services are added.</param>
    /// <param name="configuration">The application configuration instance used to retrieve settings for health checks.</param>
    public static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaHost = configuration.GetSection(nameof(KafkaSettings)).GetValue<string>(nameof(KafkaSettings.Host));
        var keycloakSettings = configuration.GetSection(nameof(KeycloakSettings)).Get<KeycloakSettings>()!;
        var redisSettings = configuration.GetSection(nameof(RedisSettings)).Get<RedisSettings>()!;
        var redisConnectionString = $"{redisSettings.Host}:{redisSettings.Port},password={redisSettings.Password}";

        var telemetrySection = configuration.GetSection(AppStartupSectionName).GetSection(TelemetrySectionName);
        var elasticSearchUrl = telemetrySection.GetValue<string>(ElasticSearchUrlName)!;
        var logstashUrl = telemetrySection.GetValue<string>(LogstashUrlName)!;
        var prometheusUrl = telemetrySection.GetValue<string>(PrometheusUrlName)!;
        var jaegerUrl = telemetrySection.GetValue<string>(JaegerHealthCheckUrlName)!;
        var aspireDashboardUrl = telemetrySection.GetValue<string>(AspireDashboardHealthCheckUrlName)!;

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>()
            .AddKafka(new ProducerConfig { BootstrapServers = kafkaHost })
            .AddElasticsearch(elasticSearchUrl)
            .AddRedis(redisConnectionString)
            .AddHangfire(options =>
            {
                options.MinimumAvailableServers = 1;
                options.MaximumJobsFailed = 10; // 10 failed jobs means the server is down
            })
            .AddUrlGroup(new Uri(prometheusUrl), "prometheus")
            .AddUrlGroup(new Uri(logstashUrl), "logstash")
            .AddUrlGroup(new Uri(keycloakSettings.Host), "keycloak")
            .AddUrlGroup(new Uri(jaegerUrl), "jaeger")
            .AddUrlGroup(new Uri(aspireDashboardUrl), "aspire");
    }

    /// <summary>
    ///     Configures Cross-Origin Resource Sharing (CORS) for the application.
    /// </summary>
    /// <param name="services">The service collection to which CORS services are added.</param>
    /// <param name="configuration">The application configuration containing the CORS settings.</param>
    /// <param name="environment">The web hosting environment used to determine development or production configuration.</param>
    public static void AddCors(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var allowedOrigins = configuration.GetSection(AppStartupSectionName).GetSection("CorsAllowedOrigins")
            .Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCorsPolicy", builder =>
            {
                if (allowedOrigins.Length > 0) builder.WithOrigins(allowedOrigins).AllowCredentials();
                else if (environment.IsDevelopment()) builder.AllowAnyOrigin();
                else
                    throw new SecurityException(
                        "No CORS origins configured. In non-development environment, at least one allowed origin must be specified.");

                builder.AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    /// <summary>
    ///     Configures the application to use localization with specified supported cultures and default request culture.
    /// </summary>
    /// <param name="app">The web application to which the localization middleware is added.</param>
    public static void UseLocalization(this IApplicationBuilder app)
    {
        app.UseRequestLocalization(options =>
        {
            string[] supportedCultures = ["en", "ru-by"];
            options.SetDefaultCulture(supportedCultures[0]);
            options.AddSupportedCultures(supportedCultures);
            options.AddSupportedUICultures(supportedCultures);
            options.ApplyCurrentCultureToResponseHeaders = true;
        });
    }

    private static IEnumerable<string> GetHosts(this IApplicationBuilder app)
    {
        HashSet<string> hosts = [];

        var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
        serverAddressesFeature?.Addresses.ToList().ForEach(x => hosts.Add(x));

        return hosts;
    }
}