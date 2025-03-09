using AutoMapper;
using UserService.Domain.Entity;

namespace UserService.Grpc.Mappings;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, GrpcUser>().ReverseMap();
    }
}