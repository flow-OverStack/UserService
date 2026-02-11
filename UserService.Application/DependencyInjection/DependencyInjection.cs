using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Mappings;
using UserService.Application.Services;
using UserService.Application.Services.Cache;
using UserService.Application.Validators;
using UserService.Domain.Dtos.Page;
using UserService.Domain.Interfaces.Service;

namespace UserService.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        services.AddAutoMapper(typeof(UserMapping));
        services.InitServices();
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<GetUserService>();
        services.AddScoped<IGetUserService, CacheGetUserService>();
        services.AddScoped<GetRoleService>();
        services.AddScoped<IGetRoleService, CacheGetRoleService>();
        services.AddScoped<IReputationService, ReputationService>();
        services.AddScoped<IUserActivityService, UserActivityService>();
        services.AddScoped<IUserActivityDatabaseService, UserActivityService>();
        services.AddScoped<GetReputationRuleService>();
        services.AddScoped<IGetReputationRuleService, CacheGetReputationRuleService>();
        services.AddScoped<GetReputationRecordService>();
        services.AddScoped<IGetReputationRecordService, CacheGetReputationRecordService>();

        services.AddScoped<IValidator<OffsetPageDto>, OffsetPageDtoValidator>();
        services.AddScoped<IValidator<CursorPageDto>, CursorPageDtoValidator>();
    }
}