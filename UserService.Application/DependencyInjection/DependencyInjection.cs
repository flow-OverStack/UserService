using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Mappings;
using UserService.Application.Services;
using UserService.Application.Services.Cache;
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
        services.Scan(scan => scan.FromAssemblyOf<AuthService>()
            .AddClasses(c => c.InExactNamespaceOf<AuthService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan.FromAssemblyOf<AuthService>()
            .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Decorate<IGetUserService, CacheGetUserService>();
        services.Decorate<IGetRoleService, CacheGetRoleService>();
        services.Decorate<IGetReputationRuleService, CacheGetReputationRuleService>();
        services.Decorate<IGetReputationRecordService, CacheGetReputationRecordService>();
    }
}