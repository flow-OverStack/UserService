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
    : IAuthService
{
    public async Task<BaseResult<UserDto>> RegisterAsync(RegisterUserDto dto,
        CancellationToken cancellationToken = default)
    {
        dto = dto with { Username = dto.Username.ToLowerInvariant() };

        var validation = await registerValidator.ValidateWithMessageAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResult<UserDto>.Failure(validation.ErrorMessage, (int)ErrorCodes.InvalidProperty);

        var user = await unitOfWork.Users.GetAll()
            .FirstOrDefaultAsync(x => x.Username == dto.Username || x.Email == dto.Email, cancellationToken);
        if (user != null)
            return BaseResult<UserDto>.Failure(ErrorMessage.UserAlreadyExists, (int)ErrorCodes.UserAlreadyExists);

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
                // Temporary IdentityId, will be updated after successful registration in IdentityServer
                IdentityId = Guid.NewGuid().ToString()
            };

            await unitOfWork.Users.CreateAsync(user, cancellationToken);
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
            return BaseResult<TokenDto>.Failure(ErrorMessage.InvalidEmail, (int)ErrorCodes.InvalidProperty);

        var user = await unitOfWork.Users.GetAll()
            .FirstOrDefaultAsync(x => x.Email == dto.Email, cancellationToken);

        return await LoginAsync(user, dto.Password, cancellationToken);
    }

    public async Task<BaseResult<UserDto>> InitAsync(InitUserDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Validate Email
        if (!IsEmail(dto.Email))
            return BaseResult<UserDto>.Failure(ErrorMessage.InvalidEmail, (int)ErrorCodes.InvalidProperty);

        // 2. Idempotency check
        var user = await unitOfWork.Users.GetAll()
            .FirstOrDefaultAsync(x => x.IdentityId == dto.IdentityId, cancellationToken);
        if (user != null)
            return BaseResult<UserDto>.Success(mapper.Map<UserDto>(user));

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
            await unitOfWork.SaveChangesAsync(cancellationToken); // user.Id populated

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
            // UpdateUsernameAsync is idempotent, so we may not roll back it
            throw;
        }

        return BaseResult<UserDto>.Success(mapper.Map<UserDto>(user));
    }

    [GeneratedRegex(@"[_\-\.]{2,}")]
    private static partial Regex UsernameRegex();

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

    private async Task<(string Username, bool IsTemporary)> ResolveUniqueUsernameAsync(
        string rawUsername,
        CancellationToken cancellationToken)
    {
        var sanitized = SanitizeUsername(rawUsername);

        // Case: invalid/empty (e.g. Chinese, special chars only)
        if (string.IsNullOrWhiteSpace(sanitized))
            return ("user", true);

        // Case: taken (short or long)
        if (await unitOfWork.Users.GetAll().AnyAsync(x => x.Username == sanitized, cancellationToken))
            return (sanitized, true);

        // Case: free
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

    private static bool IsAllowedChar(char c)
    {
        return c is >= 'a' and <= 'z' or >= '0' and <= '9' or '_' or '-' or '.';
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