using System.Net.Mail;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Enums;
using UserService.Application.Exceptions.IdentityServer.Base;
using UserService.Application.Extensions;
using UserService.Application.Resources;
using UserService.Application.Services.Provisioning;
using UserService.Domain.Dtos.Identity;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services;

public class AuthService(
    IMapper mapper,
    IIdentityServer identityServer,
    IUnitOfWork unitOfWork,
    IIdentityCompensationQueue compensationQueue,
    IUserSyncQueue userSyncQueue,
    IUserProvisioningService provisioningService,
    IValidator<RegisterUserDto> registerValidator)
    : IAuthService
{
    public async Task<BaseResult<UserDto>> RegisterAsync(RegisterUserDto dto,
        CancellationToken cancellationToken = default)
    {
        dto = dto with { Username = dto.Username.ToLowerInvariant() };

        var validation = await registerValidator.ValidateWithMessageAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResult<UserDto>.Failure(validation.ErrorMessage, (int)ErrorCodes.InvalidProperty);

        var identityUsernameUserTask = identityServer.FindUserAsync(dto.Username, cancellationToken);
        var identityEmailUserTask = identityServer.FindUserAsync(dto.Email, cancellationToken);
        var dbUserTask = unitOfWork.Users.GetAll()
            .FirstOrDefaultAsync(x => x.Username == dto.Username || x.Email == dto.Email, cancellationToken);

        await Task.WhenAll(identityUsernameUserTask, identityEmailUserTask, dbUserTask);

        var identityUsernameUser = await identityUsernameUserTask;
        var identityEmailUser = await identityEmailUserTask;
        var dbUser = await dbUserTask;

        if (identityUsernameUser != null || identityEmailUser != null || dbUser != null)
            return BaseResult<UserDto>.Failure(ErrorMessage.UserAlreadyExists,
                (int)ErrorCodes.UserAlreadyExists);

        return await CreateUserAsync(dto, cancellationToken);
    }

    public Task<BaseResult<TokenDto>> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default)
    {
        var identifier = IsEmail(dto.Identifier)
            ? dto.Identifier
            : dto.Identifier.ToLowerInvariant();

        return LoginAsync(identifier, dto.Password, cancellationToken);
    }

    public Task<BaseResult<UserDto>> InitAsync(InitUserDto dto, CancellationToken cancellationToken = default) =>
        provisioningService.InitAsync(dto, cancellationToken);

    private async Task<BaseResult<TokenDto>> LoginAsync(string identifier, string password,
        CancellationToken cancellationToken = default)
    {
        var tokenResult = await SafeLoginUserAsync(identityServer, new IdentityLoginUserDto(identifier, password),
            cancellationToken);

        if (!tokenResult.IsSuccess)
            return tokenResult;

        // Fire-and-forget: sync local user record and update LastLoginAt.
        // Zero latency impact on the login response.
        userSyncQueue.EnqueueLoginSync(identifier);

        return tokenResult;
    }

    private async Task<BaseResult<UserDto>> CreateUserAsync(RegisterUserDto dto,
        CancellationToken cancellationToken)
    {
        User user;
        IdentityUserDto? identityResponse = null;
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var role = await unitOfWork.Roles.GetAll()
                .FirstOrDefaultAsync(x => x.Name == nameof(Roles.User), cancellationToken);
            if (role == null)
                return BaseResult<UserDto>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

            user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                LastLoginAt = DateTime.UtcNow,
                Roles = [role],
                // Temporary IdentityId, will be replaced after successful registration in the identity server.
                IdentityId = Guid.NewGuid().ToString()
            };

            await unitOfWork.Users.CreateAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var identityDto = mapper.Map<IdentityRegisterUserDto>(user);
            identityDto.Password = dto.Password;

            var registerResult = await SafeRegisterUserAsync(identityServer, identityDto, cancellationToken);
            if (!registerResult.IsSuccess)
                return BaseResult<UserDto>.Failure(registerResult.ErrorMessage!, registerResult.ErrorCode);

            identityResponse = registerResult.Data;
            user.IdentityId = identityResponse!.IdentityId;
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception) when (identityResponse != null)
        {
            compensationQueue.EnqueueIdentityUserDeletion(identityResponse.IdentityId);

            throw;
        }

        return BaseResult<UserDto>.Success(mapper.Map<UserDto>(user));
    }

    private static async Task<BaseResult<IdentityUserDto>> SafeRegisterUserAsync(IIdentityServer identityServer,
        IdentityRegisterUserDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await identityServer.RegisterUserAsync(dto, cancellationToken);
            return BaseResult<IdentityUserDto>.Success(response);
        }
        catch (IdentityServerBusinessException e)
        {
            var baseResult = e.GetBaseResult();
            return BaseResult<IdentityUserDto>.Failure(baseResult.ErrorMessage!, baseResult.ErrorCode);
        }
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

    private static bool IsEmail(string email) => MailAddress.TryCreate(email, out _);
}
