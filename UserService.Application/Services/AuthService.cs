using System.Net.Mail;
using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
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
    IUnitOfWork unitOfWork)
    : IAuthService
{
    public async Task<BaseResult<UserDto>> RegisterAsync(RegisterUserDto dto)
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
                    BackgroundJob.Enqueue(() => identityServer.RollbackRegistrationAsync(keycloakResponse.KeycloakId));

                throw;
            }
        }

        return BaseResult<UserDto>.Success(mapper.Map<UserDto>(user));
    }

    public async Task<BaseResult<TokenDto>> LoginWithUsernameAsync(LoginUsernameUserDto dto)
    {
        var user = await userRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Username == dto.Username.ToLowerInvariant());

        return await LoginAsync(user, dto.Password);
    }

    public async Task<BaseResult<TokenDto>> LoginWithEmailAsync(LoginEmailUserDto dto)
    {
        if (!IsEmail(dto.Email))
            return BaseResult<TokenDto>.Failure(ErrorMessage.EmailNotValid, (int)ErrorCodes.EmailNotValid);

        var user = await userRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        return await LoginAsync(user, dto.Password);
    }

    private async Task<BaseResult<TokenDto>> LoginAsync(User? user, string password)
    {
        if (user == null)
            return BaseResult<TokenDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var keycloakDto = mapper.Map<KeycloakLoginUserDto>(user);
        keycloakDto.Password = password;

        var keycloakSafeResponse = await SafeLoginUserAsync(identityServer, keycloakDto);
        if (!keycloakSafeResponse.IsSuccess)
            return keycloakSafeResponse;

        user.LastLoginAt = DateTime.UtcNow;
        userRepository.Update(user);
        await userRepository.SaveChangesAsync();

        return BaseResult<TokenDto>.Success(keycloakSafeResponse.Data);
    }

    private static bool IsEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static async Task<BaseResult<TokenDto>> SafeLoginUserAsync(IIdentityServer identityServer,
        KeycloakLoginUserDto dto)
    {
        try
        {
            var response = await identityServer.LoginUserAsync(dto);
            return BaseResult<TokenDto>.Success(response);
        }
        catch (IdentityServerBusinessException e)
        {
            var baseResult = e.GetBaseResult();
            return BaseResult<TokenDto>.Failure(baseResult.ErrorMessage!, baseResult.ErrorCode);
        }
    }
}