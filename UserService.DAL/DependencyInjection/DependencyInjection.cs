using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.DAL.Interceptors;
using UserService.DAL.Repositories;
using UserService.Domain.Entity;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.DAL.DependencyInjection;

public static class DependencyInjection
{
    public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration,
        bool isDevelopment = true)
    {
        var connectionString = isDevelopment
            ? configuration.GetConnectionString("PostgresSQL:Development")
            : configuration.GetConnectionString("PostgresSQL:Production");
        services.AddSingleton<DateInterceptor>();
        services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(connectionString); });

        services.InitRepositories();
    }

    private static void InitRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
        services.AddScoped<IBaseRepository<Role>, BaseRepository<Role>>();
        services.AddScoped<IBaseRepository<UserRole>, BaseRepository<UserRole>>();
        services.AddScoped<IBaseRepository<Badge>, BaseRepository<Badge>>();
        services.AddScoped<IBaseRepository<UserBadge>, BaseRepository<UserBadge>>();
    }
}