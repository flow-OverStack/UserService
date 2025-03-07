using AutoMapper;

namespace UserService.Grpc.Mappings;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<Domain.Entity.User, User>().ReverseMap();
    }
}