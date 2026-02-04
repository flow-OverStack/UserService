using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using UserService.Api;
using UserService.Api.Middlewares;
using UserService.Application.DependencyInjection;
using UserService.Application.Settings;
using UserService.BackgroundJobs.DependencyInjection;
using UserService.Cache.DependencyInjection;
using UserService.Cache.Settings;
using UserService.DAL.DependencyInjection;
using UserService.Domain.Settings;
using UserService.GraphQl.DependencyInjection;
using UserService.GrpcServer.DependencyInjection;
using UserService.Keycloak.DependencyInjection;
using UserService.Keycloak.Settings;
using UserService.Messaging.DependencyInjection;
using UserService.Messaging.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KeycloakSettings>(builder.Configuration.GetSection(nameof(KeycloakSettings)));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(nameof(RedisSettings)));
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(nameof(KafkaSettings)));
builder.Services.Configure<PaginationRules>(builder.Configuration.GetSection(nameof(PaginationRules)));
builder.Services.Configure<ReputationRules>(builder.Configuration.GetSection(nameof(ReputationRules)));

builder.Services.ConfigureHttpClientDefaults(clientBuilder => clientBuilder.AddStandardResilienceHandler());
builder.Services.AddLocalization(options => options.ResourcesPath = nameof(UserService.Application.Resources));

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthenticationAndAuthorization();
builder.Services.AddIdentityServer();
builder.Services.AddSwagger();
builder.Services.AddGraphQl();
builder.Services.AddGrpcServices();
builder.Services.AddMassTransitServices();
builder.Services.AddHangfire(builder.Configuration);

builder.Host.AddLogging(builder.Configuration);

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddCache();
builder.Services.AddApplication();

builder.AddOpenTelemetry();
builder.Services.AddHealthChecks(builder.Configuration);
builder.WebHost.ConfigurePorts(builder.Configuration);
builder.Services.AddCors(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseStatusCodePages();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<WarningHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    await app.Services.MigrateDatabaseAsync();
}

app.UseSwagger();

app.UseRouting();
app.MapControllers();
app.UseLocalization();
app.UseGraphQl();
app.UseGrpcServices();
app.UseHangfire();
app.SetupHangfireJobs();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapHealthChecks("health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
app.UseCors("DefaultCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.LogListeningUrls();

await app.RunAsync();