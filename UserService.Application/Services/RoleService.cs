using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Enums;
using UserService.Application.Resources;
using UserService.Domain.Dtos.Identity.Role;
using UserService.Domain.Dtos.Role;
using UserService.Domain.Dtos.UserRole;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services;

public class RoleService(
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IIdentityServer identityServer)
    : IRoleService
{
    public async Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto,
        CancellationToken cancellationToken = default)
    {
        var role = await unitOfWork.Roles.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name, cancellationToken);
        if (role != null)
            return BaseResult<RoleDto>.Failure(ErrorMessage.RoleAlreadyExists, (int)ErrorCodes.RoleAlreadyExists);

        role = new Role
        {
            Name = dto.Name
        };
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

            await UpdateRolesAsync(usersWithRoleToDelete.Select(x => new User
            {
                Id = x.Id,
                IdentityId = x.IdentityId,
                Email = x.Email,
                Roles = x.Roles.Where(y => y.Id != role.Id).ToList()
            }), cancellationToken);
            areRolesSynced = true;

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);

            if (!areRolesSynced) throw;

            RollbackRoles(usersWithRoleToDelete);

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
            await UpdateRolesAsync(usersWithUpdatedRole, cancellationToken);
            areRolesSynced = true;

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);

            if (!areRolesSynced) throw;

            var usersWithOldRole = await GetUsersWithRoleAsync(role.Id, CancellationToken.None);
            RollbackRoles(usersWithOldRole);

            throw;
        }

        return BaseResult<RoleDto>.Success(mapper.Map<RoleDto>(role));
    }

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

            await UpdateRolesAsync(user, cancellationToken);
            areRolesSynced = true;

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);

            if (!areRolesSynced) throw;

            var userWithOldRole = await GetUserWithRolesByIdAsync(user.Id, CancellationToken.None);
            RollbackRoles(userWithOldRole!);

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

            await UpdateRolesAsync(user, cancellationToken);
            areRolesSynced = true;

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);

            if (!areRolesSynced) throw;

            var userWithOldRole = await GetUserWithRolesByIdAsync(user.Id, CancellationToken.None);
            RollbackRoles(userWithOldRole!);

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

        var newRole = await unitOfWork.Roles.GetAll().FirstOrDefaultAsync(x => x.Id == dto.ToRoleId, cancellationToken);

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

            await UpdateRolesAsync(user, cancellationToken);
            areRolesSynced = true;

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);

            if (!areRolesSynced) throw;

            var userWithOldRole = await GetUserWithRolesByIdAsync(user.Id, CancellationToken.None);
            RollbackRoles(userWithOldRole!);

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

    private async Task<IEnumerable<User>> GetUsersWithRoleAsync(long roleId,
        CancellationToken cancellationToken = default)
    {
        return await unitOfWork.Users.GetAll()
            .Include(x => x.Roles)
            .Where(x => x.Roles.Any(y => y.Id == roleId))
            .ToArrayAsync(cancellationToken);
    }

    private Task UpdateRolesAsync(User user, CancellationToken cancellationToken = default)
    {
        return UpdateRolesAsync([user], cancellationToken);
    }

    private async Task UpdateRolesAsync(IEnumerable<User> users, CancellationToken cancellationToken = default)
    {
        var updateTasks = users.Select(user =>
        {
            var dto = mapper.Map<IdentityUpdateRolesDto>(user);
            return identityServer.UpdateRolesAsync(dto, cancellationToken);
        });

        await Task.WhenAll(updateTasks);
    }

    private void RollbackRoles(User user)
    {
        RollbackRoles([user]);
    }

    private void RollbackRoles(IEnumerable<User> users)
    {
        foreach (var user in users)
        {
            var dto = mapper.Map<IdentityUpdateRolesDto>(user);
            dto.NewRoles.ForEach(x => x.Users = null!); //Removing loop dependencies
            BackgroundJob.Enqueue(() => identityServer.RollbackUpdateRolesAsync(dto));
        }
    }
}