using AutoMapper;
using UserService.Domain.Entity;

namespace UserService.Grpc.Mappings;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<Role, GrpcRole>().ReverseMap();
    }
}