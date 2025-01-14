using AutoMapper;
using UserService.Domain.Dto.Keycloak.User;
using UserService.Domain.Dto.User;
using UserService.Domain.Entity;

namespace UserService.Application.Mapping;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, KeycloakRegisterUserDto>().ReverseMap();
        CreateMap<User, KeycloakLoginUserDto>().ReverseMap();
    }
}