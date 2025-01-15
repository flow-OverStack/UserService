using AutoMapper;
using UserService.Domain.Dto.Keycloak.Token;
using UserService.Domain.Dto.Token;

namespace UserService.Application.Mapping;

public class TokenMapping : Profile
{
    public TokenMapping()
    {
        CreateMap<KeycloakUserTokenDto, TokenDto>().ReverseMap();
    }
}