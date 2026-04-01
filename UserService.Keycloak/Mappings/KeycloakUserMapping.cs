using AutoMapper;
using UserService.Domain.Dtos.Identity;
using UserService.Keycloak.HttpModels;

namespace UserService.Keycloak.Mappings;

public class KeycloakUserMapping : Profile
{
    public KeycloakUserMapping()
    {
        CreateMap<IdentityUserDto, KeycloakUser>().ReverseMap()
            .ForCtorParam(nameof(IdentityUserDto.IdentityId), x => x.MapFrom(y => y.Id));
    }
}