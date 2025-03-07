using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using UserService.Grpc.Mappings;
using UserService.Grpc.Services;

namespace UserService.Grpc.DependencyInjection;

public static class DependencyInjection
{
    public static void AddGrpcServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(UserMapping));
        services.AddGrpc();
    }

    public static void UseGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<GrpcUserService>();
    }
}