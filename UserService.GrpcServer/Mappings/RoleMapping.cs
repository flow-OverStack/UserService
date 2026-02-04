using AutoMapper;
using UserService.Domain.Entities;

namespace UserService.GrpcServer.Mappings;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<Role, GrpcRole>().ReverseMap();
    }
}