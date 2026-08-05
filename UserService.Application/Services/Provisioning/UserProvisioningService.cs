using System.Net.Mail;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Enums;
using UserService.Application.Resources;
using UserService.Domain.Dtos.Identity;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;
using UserService.Domain.Settings;

namespace UserService.Application.Services.Provisioning;

public class UserProvisioningService(
    IMapper mapper,
    IIdentityServer identityServer,
    IUnitOfWork unitOfWork,
    IUsernameGenerator usernameGenerator)
    : IUserProvisioningService, IUserSyncService
{
    public async Task<BaseResult<UserDto>> InitAsync(InitUserDto dto, CancellationToken cancellationToken = default)
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

    private async Task<BaseResult<User>> InitUserAsync(InitUserDto dto, CancellationToken cancellationToken)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var role = await unitOfWork.Roles.GetAll()
            .FirstOrDefaultAsync(x => x.Name == nameof(Roles.User), cancellationToken);
        if (role == null)
            return BaseResult<User>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

        var (usernameBase, isTemporary) = await usernameGenerator.ResolveUniqueUsernameAsync(dto.Username,
            cancellationToken);
        var username = isTemporary
            ? Guid.NewGuid().ToString("N")[..EntityConstraints.UsernameMaxLength]
            : usernameBase;

        var user = new User
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

        return BaseResult<User>.Success(user);
    }

    private static bool IsEmail(string email) => MailAddress.TryCreate(email, out _);
}
