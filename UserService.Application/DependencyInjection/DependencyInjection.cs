using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Mappings;
using UserService.Application.Services;
using UserService.Application.Validators;
using UserService.Domain.Dtos.Request.Page;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Interfaces.Validation;

namespace UserService.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(UserMapping));

        InitServices(services);
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IGetUserService, GetUserService>();
        services.AddScoped<IGetRoleService, GetRoleService>();
        services.AddScoped<IReputationService, ReputationService>();
        services.AddScoped<IReputationResetService, ReputationService>();
        services.AddScoped<IProcessedEventsResetService, ProcessedEventsResetService>();

        services.AddScoped<INullSafeValidator<OffsetPageDto>, OffsetPageDtoValidator>();
    }
}