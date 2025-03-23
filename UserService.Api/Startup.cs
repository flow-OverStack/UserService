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
using Serilog;
using UserService.BackgroundJobs;
using UserService.Domain.Settings;
using Path = System.IO.Path;

namespace UserService.Api;

/// <summary>
///     Class with methods for app stratup
/// </summary>
public static class Startup
{
    private const string AppStartupSectionName = "AppStartupSettings";
    private const string AppPortsSectionName = "Ports";

    /// <summary>
    ///     Sets up authentication and authorization
    /// </summary>
    /// <param name="services"></param>
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
    ///     Swagger set up
    /// </summary>
    /// <param name="services"></param>
    public static void AddSwagger(this IServiceCollection services)
    {
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
            options.SwaggerDoc("v1", new OpenApiInfo
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
                    Array.Empty<string>()
                }
            });

            var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
        });
    }

    /// <summary>
    ///     Logs listening urls
    /// </summary>
    /// <param name="app"></param>
    public static void LogListeningUrls(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var hosts = app.GetHosts().ToList();

            var appStartupHostLog =
                app.Configuration.GetSection(AppStartupSectionName).GetValue<string>("AppStartupUrlLog");

            hosts.ForEach(host =>
            {
                var fullHostLog = appStartupHostLog + host;
                Log.Information(fullHostLog);
            });
        });
    }

    /// <summary>
    ///     Configure ports for application
    /// </summary>
    /// <param name="builder"></param>
    public static void ConfigurePorts(this WebApplicationBuilder builder)
    {
        var grpcPort = builder.Configuration.GetSection(AppStartupSectionName).GetSection(AppPortsSectionName)
            .GetValue<int>("GrpcPort");

        var apiPort = builder.Configuration.GetSection(AppStartupSectionName).GetSection(AppPortsSectionName)
            .GetValue<int>("RestApiPort");

        var useHttpsForApi = builder.Configuration.GetSection(AppStartupSectionName).GetSection(AppPortsSectionName)
            .GetValue<bool>("UseHttpsForRestApi");

        builder.WebHost.ConfigureKestrel(opt =>
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
    ///     Configures hangfire and adds jobs
    /// </summary>
    /// <param name="builder"></param>
    public static void AddHangfire(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(x => x.UsePostgreSqlStorage(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("PostgresSQL");
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

        builder.Services.AddHangfireServer();

        builder.Services.InitJobs();
    }

    private static IEnumerable<string> GetHosts(this WebApplication app)
    {
        HashSet<string> hosts = [];

        var serverAddressesFeature = ((IApplicationBuilder)app).ServerFeatures.Get<IServerAddressesFeature>();
        serverAddressesFeature?.Addresses.ToList().ForEach(x => hosts.Add(x));

        return hosts;
    }
}