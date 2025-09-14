using System.Net.Mail;
using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Enums;
using UserService.Application.Exceptions.IdentityServer.Base;
using UserService.Application.Resources;
using UserService.Domain.Dtos.Identity.User;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services;

public class AuthService(
    IMapper mapper,
    IIdentityServer identityServer,
    IUnitOfWork unitOfWork,
    IBackgroundJobClient backgroundJob)
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

        IdentityUserDto? identityResponse = null;
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            user = new User
            {
                Username = lowerUsername,
                Email = dto.Email,
                LastLoginAt = DateTime.UtcNow,
                IdentityId = "PENDING"
            };

            await unitOfWork.Users.CreateAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var role = await unitOfWork.Roles.GetAll()
                .FirstOrDefaultAsync(x => x.Name == nameof(Roles.User), cancellationToken);
            if (role == null)
                return BaseResult<UserDto>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

            user.Roles = [role];
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var identityDto = mapper.Map<IdentityRegisterUserDto>(user);
            identityDto.Password = dto.Password;

            identityResponse = await identityServer.RegisterUserAsync(identityDto, cancellationToken);
            user.IdentityId = identityResponse.IdentityId;
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            if (identityResponse != null)
                backgroundJob.Enqueue<IIdentityServer>(server =>
                    server.RollbackRegistrationAsync(new IdentityUserDto(identityResponse.IdentityId)));

            throw;
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

    public async Task<BaseResult<UserDto>> InitAsync(InitUserDto dto, CancellationToken cancellationToken = default)
    {
        if (!IsEmail(dto.Email))
            return BaseResult<UserDto>.Failure(ErrorMessage.EmailNotValid, (int)ErrorCodes.EmailNotValid);

        var lowerUsername = dto.Username.ToLowerInvariant();

        var user = await unitOfWork.Users.GetAll()
                       .FirstOrDefaultAsync(x => x.Username == lowerUsername, cancellationToken) ??
                   await unitOfWork.Users.GetAll().FirstOrDefaultAsync(x => x.Email == dto.Email, cancellationToken);
        if (user != null)
            return BaseResult<UserDto>.Failure(ErrorMessage.UserAlreadyExists, (int)ErrorCodes.UserAlreadyExists);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            user = new User
            {
                Username = lowerUsername,
                Email = dto.Email,
                LastLoginAt = DateTime.UtcNow,
                IdentityId = dto.IdentityId
            };

            await unitOfWork.Users.CreateAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var role = await unitOfWork.Roles.GetAll()
                .FirstOrDefaultAsync(x => x.Name == nameof(Roles.User), cancellationToken);
            if (role == null)
                return BaseResult<UserDto>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

            user.Roles = [role];
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }

        return BaseResult<UserDto>.Success(mapper.Map<UserDto>(user));
    }

    private async Task<BaseResult<TokenDto>> LoginAsync(User? user, string password,
        CancellationToken cancellationToken = default)
    {
        if (user == null)
            return BaseResult<TokenDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var identityDto = new IdentityLoginUserDto(user.Username, password);

        var identitySafeResponse = await SafeLoginUserAsync(identityServer, identityDto, cancellationToken);
        if (!identitySafeResponse.IsSuccess)
            return identitySafeResponse;

        user.LastLoginAt = DateTime.UtcNow;
        unitOfWork.Users.Update(user);
        await unitOfWork.Users.SaveChangesAsync(cancellationToken);

        return BaseResult<TokenDto>.Success(identitySafeResponse.Data);
    }

    private static bool IsEmail(string email)
    {
        return MailAddress.TryCreate(email, out _);
    }

    private static async Task<BaseResult<TokenDto>> SafeLoginUserAsync(IIdentityServer identityServer,
        IdentityLoginUserDto dto, CancellationToken cancellationToken = default)
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