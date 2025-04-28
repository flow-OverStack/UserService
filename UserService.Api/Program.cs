using Serilog;
using UserService.Api;
using UserService.Api.Middlewares;
using UserService.Application.DependencyInjection;
using UserService.BackgroundJobs;
using UserService.DAL.DependencyInjection;
using UserService.Domain.Settings;
using UserService.GraphQl.DependencyInjection;
using UserService.Grpc.DependencyInjection;
using UserService.Keycloak.DependencyInjection;
using UserService.ReputationConsumer.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KeycloakSettings>(builder.Configuration.GetSection(nameof(KeycloakSettings)));
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(nameof(KafkaSettings)));
builder.Services.Configure<BusinessRules>(builder.Configuration.GetSection(nameof(BusinessRules)));

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

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication();

builder.WebHost.ConfigurePorts(builder.Configuration);

var app = builder.Build();

app.UseStatusCodePages();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<WarningHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
app.UseGraphQl();
app.UseGrpcServices();
app.SetupHangfire();

app.UseAuthentication();
app.UseAuthorization();

await builder.Services.MigrateDatabaseAsync();

app.LogListeningUrls();

await app.RunAsync();

public partial class Program
{
}