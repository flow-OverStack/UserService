using AutoMapper;
using UserService.Domain.Entities;

namespace UserService.Grpc.Mappings;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<Role, GrpcRole>().ReverseMap();
    }
}