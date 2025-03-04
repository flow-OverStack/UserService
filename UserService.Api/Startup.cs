using System.Reflection;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using UserService.Api.Authorization;
using UserService.Domain.Settings;
using Path = System.IO.Path;

namespace UserService.Api;

/// <summary>
///     Class with methods for app stratup
/// </summary>
public static class Startup
{
    private const string AppStartupSectionName = "AppStartupSettings";

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
            options.Audience = keycloakSettings.Audience; //Default audience
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidAudiences =
                    [keycloakSettings.Audience, keycloakSettings.ServiceAudience], //Additional valid service audience
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });

        services.AddAuthorization(options =>
        {
            var keycloakSettings =
                services.BuildServiceProvider().GetRequiredService<IOptions<KeycloakSettings>>().Value;
            var serviceAudience = keycloakSettings.ServiceAudience;
            var userAudience = keycloakSettings.Audience;

            options.AddPolicy("DefaultOrServiceApi",
                builder => builder.Requirements.Add(new AudienceRequirement(serviceAudience, userAudience)));
        });

        services.AddSingleton<IAuthorizationHandler, AudienceAuthorizationHandler>();
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
            HashSet<string> hosts = [];

            var serverAddressesFeature = ((IApplicationBuilder)app).ServerFeatures.Get<IServerAddressesFeature>();
            serverAddressesFeature?.Addresses.ToList().ForEach(x => hosts.Add(x));

            var serverAddressesConfiguration = app.Configuration.GetSection("ASPNETCORE_URLS");
            serverAddressesConfiguration.Value?.Split(';').ToList().ForEach(x => hosts.Add(x));

            var appStartupHostLog =
                app.Configuration.GetSection(AppStartupSectionName).GetValue<string>("AppStartupUrlLog");

            hosts.ToList().ForEach(host =>
            {
                var fullHostLog = appStartupHostLog + host;
                Log.Information(fullHostLog);
            });
        });
    }
}