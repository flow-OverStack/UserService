using AutoMapper;
using UserService.Domain.Dtos.Identity.Role;
using UserService.Domain.Dtos.Identity.User;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;

namespace UserService.Application.Mappings;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, IdentityRegisterUserDto>().ReverseMap();
        CreateMap<User, IdentityLoginUserDto>().ReverseMap();
        CreateMap<IdentityUpdateRolesDto, User>().ReverseMap()
            .ForMember(x => x.UserId, x => x.MapFrom(y => y.Id))
            .ForMember(x => x.KeycloakUserId, x => x.MapFrom(y => y.KeycloakId))
            .ForMember(x => x.NewRoles, x => x.MapFrom(y => y.Roles));
    }
}