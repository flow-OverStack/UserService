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
            .ForCtorParam(nameof(IdentityUpdateRolesDto.UserId), x => x.MapFrom(y => y.Id))
            .ForCtorParam(nameof(IdentityUpdateRolesDto.NewRoles), x => x.MapFrom(y => y.Roles));
    }
}