using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Enums;
using UserService.Application.Resources;
using UserService.Application.Services.Identity;
using UserService.Domain.Dtos.Role;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services;

public class RoleService(
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IIdentityRoleSynchronizer synchronizer)
    : IRoleService
{
    public async Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        var role = await unitOfWork.Roles.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name, cancellationToken);
        if (role != null)
            return BaseResult<RoleDto>.Failure(ErrorMessage.RoleAlreadyExists, (int)ErrorCodes.RoleAlreadyExists);

        role = new Role { Name = dto.Name };
        await unitOfWork.Roles.CreateAsync(role, cancellationToken);
        await unitOfWork.Roles.SaveChangesAsync(cancellationToken);

        return BaseResult<RoleDto>.Success(mapper.Map<RoleDto>(role));
    }

    public async Task<BaseResult<RoleDto>> DeleteRoleAsync(long id, CancellationToken cancellationToken = default)
    {
        var role = await unitOfWork.Roles.GetAll().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (role == null)
            return BaseResult<RoleDto>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

        if (role.Name == nameof(Roles.User))
            return BaseResult<RoleDto>.Failure(ErrorMessage.CannotDeleteDefaultRole,
                (int)ErrorCodes.CannotDeleteDefaultRole);

        var usersWithRoleToDelete = (await GetUsersWithRoleAsync(role.Id, cancellationToken)).ToArray();

        var areRolesSynced = false;
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            unitOfWork.Roles.Remove(role);
            await unitOfWork.Roles.SaveChangesAsync(cancellationToken);

            await synchronizer.SyncAsync(usersWithRoleToDelete.Select(x => new User
            {
                Id = x.Id,
                IdentityId = x.IdentityId,
                Email = x.Email,
                Roles = x.Roles.Where(y => y.Id != role.Id).ToList()
            }), cancellationToken);
            areRolesSynced = true;

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception) when (areRolesSynced)
        {
            synchronizer.ScheduleCompensation(usersWithRoleToDelete);
            throw;
        }

        return BaseResult<RoleDto>.Success(mapper.Map<RoleDto>(role));
    }

    public async Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = await unitOfWork.Roles.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);
        if (role == null)
            return BaseResult<RoleDto>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

        var areRolesSynced = false;
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            role.Name = dto.Name;
            unitOfWork.Roles.Update(role);
            await unitOfWork.Roles.SaveChangesAsync(cancellationToken);

            var usersWithUpdatedRole = await GetUsersWithRoleAsync(role.Id, cancellationToken);
            await synchronizer.SyncAsync(usersWithUpdatedRole, cancellationToken);
            areRolesSynced = true;

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception) when (areRolesSynced)
        {
            var usersWithOldRole = await GetUsersWithRoleAsync(role.Id, CancellationToken.None);
            synchronizer.ScheduleCompensation(usersWithOldRole);
            throw;
        }

        return BaseResult<RoleDto>.Success(mapper.Map<RoleDto>(role));
    }

    private async Task<IEnumerable<User>> GetUsersWithRoleAsync(long roleId,
        CancellationToken cancellationToken = default)
    {
        return await unitOfWork.Users.GetAll()
            .Include(x => x.Roles)
            .Where(x => x.Roles.Any(y => y.Id == roleId))
            .ToArrayAsync(cancellationToken);
    }
}
