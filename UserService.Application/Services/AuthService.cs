using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Dto.Keycloak.Token;
using UserService.Domain.Dto.Keycloak.User;
using UserService.Domain.Dto.Token;
using UserService.Domain.Dto.User;
using UserService.Domain.Entity;
using UserService.Domain.Enum;
using UserService.Domain.Exceptions.IdentityServer.Base;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Resources;
using UserService.Domain.Result;

namespace UserService.Application.Services;

public class AuthService(
    IBaseRepository<User> userRepository,
    IMapper mapper,
    IIdentityServer identityServer,
    IBaseRepository<Role> roleRepository,
    IUnitOfWork unitOfWork,
    IBaseRepository<UserToken> userTokenRepository)
    : IAuthService
{
    public async Task<BaseResult<UserDto>> Register(RegisterUserDto dto)
    {
        if (!IsEmail(dto.Email))
            return BaseResult<UserDto>.Failure(ErrorMessage.EmailNotValid, (int)ErrorCodes.EmailNotValid);

        var lowerUsername = dto.Username.ToLowerInvariant();

        var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Username == lowerUsername) ??
                   await userRepository.GetAll().FirstOrDefaultAsync(x => x.Email == dto.Email);
        if (user != null)
            return BaseResult<UserDto>.Failure(ErrorMessage.UserAlreadyExists, (int)ErrorCodes.UserAlreadyExists);

        KeycloakUserDto? keycloakResponse = null;
        await using (var transaction = await unitOfWork.BeginTransactionAsync())
        {
            try
            {
                user = new User
                {
                    Username = lowerUsername,
                    Email = dto.Email,
                    LastLoginAt = DateTime.UtcNow
                };

                await unitOfWork.Users.CreateAsync(user);
                await unitOfWork.SaveChangesAsync();

                var role = await roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == nameof(Roles.User));
                if (role == null)
                    return BaseResult<UserDto>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                };
                await unitOfWork.UserRoles.CreateAsync(userRole);
                await unitOfWork.SaveChangesAsync();

                var keycloakDto = mapper.Map<KeycloakRegisterUserDto>(user);
                keycloakDto.Password = dto.Password;

                keycloakResponse = await identityServer.RegisterUserAsync(keycloakDto);
                user.KeycloakId = keycloakResponse.KeycloakId;
                await unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                if (keycloakResponse != null)
                    await identityServer.RollbackRegistration(keycloakResponse.KeycloakId);

                throw;
            }
        }

        return BaseResult<UserDto>.Success(mapper.Map<UserDto>(user));
    }

    public async Task<BaseResult<TokenDto>> LoginWithUsername(LoginUsernameUserDto dto)
    {
        var user = await userRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Username == dto.Username.ToLowerInvariant());

        return await Login(user, dto.Password);
    }

    public async Task<BaseResult<TokenDto>> LoginWithEmail(LoginEmailUserDto dto)
    {
        if (!IsEmail(dto.Email))
            return BaseResult<TokenDto>.Failure(ErrorMessage.EmailNotValid, (int)ErrorCodes.EmailNotValid);

        var user = await userRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        return await Login(user, dto.Password);
    }

    private async Task<BaseResult<TokenDto>> Login(User? user, string password)
    {
        if (user == null)
            return BaseResult<TokenDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var userToken = await userTokenRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);
        var keycloakDto = mapper.Map<KeycloakLoginUserDto>(user);
        keycloakDto.Password = password;

        var keycloakSafeResponse = await SafeLoginUser(identityServer, keycloakDto);
        if (!keycloakSafeResponse.IsSuccess)
            return BaseResult<TokenDto>.Failure(keycloakSafeResponse.ErrorMessage!, keycloakSafeResponse.ErrorCode);

        var keycloakResponse = keycloakSafeResponse.Data;

        if (userToken == null)
        {
            userToken = new UserToken
            {
                UserId = user.Id,
                RefreshToken = keycloakResponse.RefreshToken,
                RefreshTokenExpiryTime = keycloakResponse.RefreshExpires
            };
            await userTokenRepository.CreateAsync(userToken);
        }
        else
        {
            userToken.RefreshToken = keycloakResponse.RefreshToken;
            userToken.RefreshTokenExpiryTime = keycloakResponse.RefreshExpires;
            userTokenRepository.Update(userToken);
        }

        await userTokenRepository.SaveChangesAsync();
        user.LastLoginAt = DateTime.UtcNow;
        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        var tokenDto = mapper.Map<TokenDto>(keycloakResponse);
        tokenDto.UserId = user.Id;

        return BaseResult<TokenDto>.Success(tokenDto);
    }

    private static bool IsEmail(string email)
    {
        var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return emailRegex.IsMatch(email);
    }

    private static async Task<BaseResult<KeycloakUserTokenDto>> SafeLoginUser(IIdentityServer identityServer,
        KeycloakLoginUserDto userDto)
    {
        try
        {
            var response = await identityServer.LoginUserAsync(userDto);
            return BaseResult<KeycloakUserTokenDto>.Success(response);
        }
        catch (IdentityServerBusinessException e)
        {
            var baseResult = e.GetBaseResult();
            return BaseResult<KeycloakUserTokenDto>.Failure(baseResult.ErrorMessage!, baseResult.ErrorCode);
        }
    }
}