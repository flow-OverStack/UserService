using AutoMapper;
using UserService.Domain.Dtos.Role;
using UserService.Domain.Entities;

namespace UserService.Application.Mappings;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<Role, RoleDto>().ReverseMap();
    }
}