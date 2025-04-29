using AutoMapper;
using UserService.Domain.Entities;

namespace UserService.Grpc.Mappings;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, GrpcUser>().ReverseMap();
    }
}