using System.Net.Mail;
using System.Text.RegularExpressions;
using AutoMapper;
using FluentValidation;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Enums;
using UserService.Application.Exceptions.IdentityServer.Base;
using UserService.Application.Helpers;
using UserService.Application.Resources;
using UserService.Domain.Dtos.Identity;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;
using UserService.Domain.Settings;

namespace UserService.Application.Services;

public partial class AuthService(
    IMapper mapper,
    IIdentityServer identityServer,
    IUnitOfWork unitOfWork,
    IBackgroundJobClient backgroundJob,
    IValidator<RegisterUserDto> registerValidator)
    : IAuthService, IUserSyncService
{
    public async Task<BaseResult<UserDto>> RegisterAsync(RegisterUserDto dto,
        CancellationToken cancellationToken = default)
    {
        dto = dto with { Username = dto.Username.ToLowerInvariant() };

        var validation = await registerValidator.ValidateWithMessageAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResult<UserDto>.Failure(validation.ErrorMessage, (int)ErrorCodes.InvalidProperty);

        var identityUser = await identityServer.FindUserAsync(dto.Username, cancellationToken);

        if (identityUser != null)
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

    public async Task<BaseResult<UserDto>> InitAsync(InitUserDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!IsEmail(dto.Email))
            return BaseResult<UserDto>.Failure(ErrorMessage.InvalidEmail, (int)ErrorCodes.InvalidProperty);

        var user = await unitOfWork.Users.GetAll()
            .FirstOrDefaultAsync(x => x.IdentityId == dto.IdentityId, cancellationToken);
        if (user != null)
            return BaseResult<UserDto>.Success(mapper.Map<UserDto>(user));

        var result = await InitUserAsync(dto, cancellationToken);
        if (!result.IsSuccess)
            return BaseResult<UserDto>.Failure(result.ErrorMessage!, result.ErrorCode);

        return BaseResult<UserDto>.Success(mapper.Map<UserDto>(result.Data));
    }

    public async Task SyncUserOnLoginAsync(string identifier, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetAll()
            .FirstOrDefaultAsync(x => x.Username == identifier || x.Email == identifier, cancellationToken);

        if (user == null)
        {
            // Local record is missing — create it from the identity server data.
            var identityUser = await identityServer.FindUserAsync(identifier, cancellationToken);
            if (identityUser == null) return;

            var initDto = mapper.Map<InitUserDto>(identityUser);
            await InitUserAsync(initDto, cancellationToken);
            return;
        }

        user.LastLoginAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }


    private async Task<BaseResult<TokenDto>> LoginAsync(string identifier, string password,
        CancellationToken cancellationToken = default)
    {
        var tokenResult = await SafeLoginUserAsync(identityServer, new IdentityLoginUserDto(identifier, password),
            cancellationToken);

        if (!tokenResult.IsSuccess)
            return tokenResult;

        // Fire-and-forget: sync local user record and update LastLoginAt.
        // Zero latency impact on the login response.
        backgroundJob.Enqueue<IUserSyncService>(svc =>
            svc.SyncUserOnLoginAsync(identifier, CancellationToken.None));

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
            {
                await transaction.RollbackAsync(CancellationToken.None);
                return BaseResult<UserDto>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);
            }

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
            {
                await transaction.RollbackAsync(CancellationToken.None);
                return BaseResult<UserDto>.Failure(registerResult.ErrorMessage!, registerResult.ErrorCode);
            }

            identityResponse = registerResult.Data;
            user.IdentityId = identityResponse!.IdentityId;
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);

            if (identityResponse != null)
                backgroundJob.Enqueue<IIdentityServer>(server =>
                    server.DeleteUserAsync(new IdentityUserIdDto(identityResponse.IdentityId)));

            throw;
        }

        return BaseResult<UserDto>.Success(mapper.Map<UserDto>(user));
    }

    private async Task<BaseResult<User>> InitUserAsync(InitUserDto dto, CancellationToken cancellationToken)
    {
        User user;
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var role = await unitOfWork.Roles.GetAll()
                .FirstOrDefaultAsync(x => x.Name == nameof(Roles.User), cancellationToken);
            if (role == null)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                return BaseResult<User>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);
            }

            var (usernameBase, isTemporary) = await ResolveUniqueUsernameAsync(dto.Username, cancellationToken);
            var username = isTemporary
                ? Guid.NewGuid().ToString("N")[..EntityConstraints.UsernameMaxLength]
                : usernameBase;

            user = new User
            {
                Username = username,
                Email = dto.Email,
                LastLoginAt = DateTime.UtcNow,
                IdentityId = dto.IdentityId,
                Roles = [role]
            };

            await unitOfWork.Users.CreateAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            if (isTemporary)
            {
                var suffix = $"_{user.Id}";
                var baseLength = Math.Min(usernameBase.Length, EntityConstraints.UsernameMaxLength - suffix.Length);
                user.Username = usernameBase[..baseLength] + suffix;

                await unitOfWork.SaveChangesAsync(cancellationToken);

                var identityDto = mapper.Map<IdentityUpdateUserDto>(user);
                await identityServer.UpdateUserAsync(identityDto, cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }

        return BaseResult<User>.Success(user);
    }

    private async Task<(string Username, bool IsTemporary)> ResolveUniqueUsernameAsync(
        string rawUsername, CancellationToken cancellationToken)
    {
        var sanitized = SanitizeUsername(rawUsername);

        if (string.IsNullOrWhiteSpace(sanitized))
            return ("user", true);

        if (await unitOfWork.Users.GetAll().AnyAsync(x => x.Username == sanitized, cancellationToken))
            return (sanitized, true);

        return (sanitized, false);
    }

    private static string SanitizeUsername(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return string.Empty;

        var lower = raw.ToLowerInvariant();
        var mapped = new string(lower.Select(c => IsAllowedChar(c) ? c : '_').ToArray());
        var collapsed = UsernameRegex().Replace(mapped, "_");
        var trimmed = collapsed.Trim('_', '-', '.');

        return trimmed.Length > EntityConstraints.UsernameMaxLength
            ? trimmed[..EntityConstraints.UsernameMaxLength]
            : trimmed;
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

    private static bool IsAllowedChar(char c)
        => c is >= 'a' and <= 'z' or >= '0' and <= '9' or '_' or '-' or '.';

    private static bool IsEmail(string email) => MailAddress.TryCreate(email, out _);

    [GeneratedRegex(@"[_\-\.]{2,}")]
    private static partial Regex UsernameRegex();
}