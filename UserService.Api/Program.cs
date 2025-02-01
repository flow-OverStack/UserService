using Serilog;
using UserService.Api;
using UserService.Api.Middlewares;
using UserService.Application.DependencyInjection;
using UserService.DAL.DependencyInjection;
using UserService.Domain.Settings;
using UserService.Keycloak.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KeycloakSettings>(builder.Configuration.GetSection(nameof(KeycloakSettings)));

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthenticationAndAuthorization();
builder.Services.AddIdentityServer();
builder.Services.AddSwagger();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDataAccessLayer(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddApplication();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<WarningHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

await app.MigrateDatabaseAsync(builder.Services);

app.LogListeningUrls();

await app.RunAsync();

public partial class Program
{
}