using AutoMapper;

namespace UserService.Grpc.Mappings;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<Domain.Entity.Role, Role>().ReverseMap();
    }
}