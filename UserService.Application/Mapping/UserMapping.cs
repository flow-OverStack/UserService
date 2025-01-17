using AutoMapper;
using UserService.Domain.Dto.Keycloak.Roles;
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
        CreateMap<KeycloakUpdateRolesDto, User>().ReverseMap()
            .ForMember(x => x.UserId, x => x.MapFrom(y => y.Id))
            .ForMember(x => x.KeycloakUserId, x => x.MapFrom(y => y.KeycloakId))
            .ForMember(x => x.NewRoles, x => x.MapFrom(y => y.Roles));
    }
}