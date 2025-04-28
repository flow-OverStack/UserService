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
    IMapper mapper,
    IIdentityServer identityServer,
    IUnitOfWork unitOfWork)
    : IAuthService
{
    public async Task<BaseResult<UserDto>> RegisterAsync(RegisterUserDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!IsEmail(dto.Email))
            return BaseResult<UserDto>.Failure(ErrorMessage.EmailNotValid, (int)ErrorCodes.EmailNotValid);

        var lowerUsername = dto.Username.ToLowerInvariant();

        var user = await unitOfWork.Users.GetAll()
                       .FirstOrDefaultAsync(x => x.Username == lowerUsername, cancellationToken) ??
                   await unitOfWork.Users.GetAll().FirstOrDefaultAsync(x => x.Email == dto.Email, cancellationToken);
        if (user != null)
            return BaseResult<UserDto>.Failure(ErrorMessage.UserAlreadyExists, (int)ErrorCodes.UserAlreadyExists);

        KeycloakUserDto? keycloakResponse = null;
        await using (var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                user = new User
                {
                    Username = lowerUsername,
                    Email = dto.Email,
                    LastLoginAt = DateTime.UtcNow
                };

                await unitOfWork.Users.CreateAsync(user, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var role = await unitOfWork.Roles.GetAll()
                    .FirstOrDefaultAsync(x => x.Name == nameof(Roles.User), cancellationToken);
                if (role == null)
                    return BaseResult<UserDto>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

                user.Roles = [role];
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var keycloakDto = mapper.Map<KeycloakRegisterUserDto>(user);
                keycloakDto.Password = dto.Password;

                keycloakResponse = await identityServer.RegisterUserAsync(keycloakDto, cancellationToken);
                user.KeycloakId = keycloakResponse.KeycloakId;
                await unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
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

    public async Task<BaseResult<TokenDto>> LoginWithUsernameAsync(LoginUsernameUserDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.Users.GetAll()
            .FirstOrDefaultAsync(x => x.Username == dto.Username.ToLowerInvariant(), cancellationToken);

        return await LoginAsync(user, dto.Password, cancellationToken);
    }

    public async Task<BaseResult<TokenDto>> LoginWithEmailAsync(LoginEmailUserDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!IsEmail(dto.Email))
            return BaseResult<TokenDto>.Failure(ErrorMessage.EmailNotValid, (int)ErrorCodes.EmailNotValid);

        var user = await unitOfWork.Users.GetAll()
            .FirstOrDefaultAsync(x => x.Email == dto.Email, cancellationToken);

        return await LoginAsync(user, dto.Password, cancellationToken);
    }

    private async Task<BaseResult<TokenDto>> LoginAsync(User? user, string password,
        CancellationToken cancellationToken = default)
    {
        if (user == null)
            return BaseResult<TokenDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var keycloakDto = mapper.Map<KeycloakLoginUserDto>(user);
        keycloakDto.Password = password;

        var keycloakSafeResponse = await SafeLoginUserAsync(identityServer, keycloakDto, cancellationToken);
        if (!keycloakSafeResponse.IsSuccess)
            return keycloakSafeResponse;

        user.LastLoginAt = DateTime.UtcNow;
        unitOfWork.Users.Update(user);
        await unitOfWork.Users.SaveChangesAsync(cancellationToken);

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
        KeycloakLoginUserDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await identityServer.LoginUserAsync(dto, cancellationToken);
            return BaseResult<TokenDto>.Success(response);
        }
        catch (IdentityServerBusinessException e)
        {
            var baseResult = e.GetBaseResult();
            return BaseResult<TokenDto>.Failure(baseResult.ErrorMessage!, baseResult.ErrorCode);
        }
    }
}