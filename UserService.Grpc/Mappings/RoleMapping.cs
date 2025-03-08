using AutoMapper;

namespace UserService.Grpc.Mappings;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<Domain.Entity.Role, Role>()
            //does not cause the stack overflow
            .ForMember(x => x.Users, x => x.Ignore())
            .ReverseMap();
    }
}