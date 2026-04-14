using AutoMapper;
using UserService.Domain.Dtos.Identity;
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
        CreateMap<IdentityUpdateUserDto, User>().ReverseMap()
            .ForCtorParam(nameof(IdentityUpdateUserDto.UserId), x => x.MapFrom(y => y.Id))
            .ForCtorParam(nameof(IdentityUpdateUserDto.Roles), x => x.MapFrom(y => y.Roles));
        CreateMap<User, UserActivityDto>()
            .ForMember(x => x.UserId, x => x.MapFrom(y => y.Id))
            .ReverseMap();
        CreateMap<User, IdentityUserDto>().ReverseMap();
        CreateMap<InitUserDto, IdentityUserDto>().ReverseMap();
    }
}