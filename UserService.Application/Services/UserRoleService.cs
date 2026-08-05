using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Enums;
using UserService.Application.Resources;
using UserService.Application.Services.Identity;
using UserService.Domain.Dtos.Identity;
using UserService.Domain.Dtos.UserRole;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services;

public class UserRoleService(
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IIdentityRoleSynchronizer synchronizer)
    : IUserRoleService
{
    public async Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.Users.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Username == dto.Username.ToLowerInvariant(), cancellationToken);

        if (user == null)
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        if (user.Roles.Any(x => x.Id == dto.RoleId))
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.UserAlreadyHasThisRole,
                (int)ErrorCodes.UserAlreadyHasThisRole);

        var role = await unitOfWork.Roles.GetAll().FirstOrDefaultAsync(x => x.Id == dto.RoleId, cancellationToken);
        if (role == null)
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

        var areRolesSynced = false;
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            user.Roles.Add(role);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await synchronizer.SyncAsync(mapper.Map<IdentitySyncSourceDto>(user), cancellationToken);
            areRolesSynced = true;

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception) when (areRolesSynced)
        {
            var userWithOldRoles = await GetUserWithRolesByIdAsync(user.Id, CancellationToken.None);
            synchronizer.ScheduleCompensation(mapper.Map<IdentitySyncSourceDto>(userWithOldRoles!));
            throw;
        }

        return BaseResult<UserRoleDto>.Success(new UserRoleDto
        {
            Username = user.Username,
            RoleId = role.Id
        });
    }

    public async Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(UserRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.Users.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Username == dto.Username.ToLowerInvariant(), cancellationToken);

        if (user == null)
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var role = user.Roles.FirstOrDefault(x => x.Id == dto.RoleId);
        if (role == null)
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

        if (role.Name == nameof(Roles.User))
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.CannotDeleteDefaultRole,
                (int)ErrorCodes.CannotDeleteDefaultRole);

        var areRolesSynced = false;
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            user.Roles.Remove(role);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await synchronizer.SyncAsync(mapper.Map<IdentitySyncSourceDto>(user), cancellationToken);
            areRolesSynced = true;

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception) when (areRolesSynced)
        {
            var userWithOldRoles = await GetUserWithRolesByIdAsync(user.Id, CancellationToken.None);
            synchronizer.ScheduleCompensation(mapper.Map<IdentitySyncSourceDto>(userWithOldRoles!));
            throw;
        }

        return BaseResult<UserRoleDto>.Success(new UserRoleDto
        {
            Username = user.Username,
            RoleId = role.Id
        });
    }

    public async Task<BaseResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.Users.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Username == dto.Username.ToLowerInvariant(), cancellationToken);

        if (user == null)
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var role = user.Roles.FirstOrDefault(x => x.Id == dto.FromRoleId);
        if (role == null)
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.RoleToBeUpdatedIsNotFound,
                (int)ErrorCodes.RoleNotFound);

        var newRole = await unitOfWork.Roles.GetAll()
            .FirstOrDefaultAsync(x => x.Id == dto.ToRoleId, cancellationToken);
        if (newRole == null)
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.RoleToUpdateIsNotFound, (int)ErrorCodes.RoleNotFound);

        if (user.Roles.Any(x => x.Id == dto.ToRoleId))
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.UserAlreadyHasThisRole,
                (int)ErrorCodes.UserAlreadyHasThisRole);

        var areRolesSynced = false;
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            user.Roles.Remove(role);
            user.Roles.Add(newRole);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await synchronizer.SyncAsync(mapper.Map<IdentitySyncSourceDto>(user), cancellationToken);
            areRolesSynced = true;

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception) when (areRolesSynced)
        {
            var userWithOldRoles = await GetUserWithRolesByIdAsync(user.Id, CancellationToken.None);
            synchronizer.ScheduleCompensation(mapper.Map<IdentitySyncSourceDto>(userWithOldRoles!));
            throw;
        }

        return BaseResult<UserRoleDto>.Success(new UserRoleDto
        {
            Username = user.Username,
            RoleId = newRole.Id
        });
    }

    private async Task<User?> GetUserWithRolesByIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.Users.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }
}
