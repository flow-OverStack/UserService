using AutoMapper;
using UserService.Domain.Dto.Role;
using UserService.Domain.Entity;

namespace UserService.Application.Mapping;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<Role, RoleDto>().ReverseMap();
    }
}