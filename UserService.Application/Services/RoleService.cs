using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Dto.Keycloak.Roles;
using UserService.Domain.Dto.Role;
using UserService.Domain.Dto.UserRole;
using UserService.Domain.Entity;
using UserService.Domain.Enum;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Resources;
using UserService.Domain.Result;

namespace UserService.Application.Services;

public class RoleService(
    IBaseRepository<User> userRepository,
    IBaseRepository<Role> roleRepository,
    IMapper mapper,
    IBaseRepository<UserRole> userRoleRepository,
    IUnitOfWork unitOfWork,
    IIdentityServer identityServer)
    : IRoleService
{
    public async Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto)
    {
        var role = await roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
        if (role != null)
            return BaseResult<RoleDto>.Failure(ErrorMessage.RoleAlreadyExists, (int)ErrorCodes.RoleAlreadyExists);

        role = new Role
        {
            Name = dto.Name
        };
        await roleRepository.CreateAsync(role);
        await roleRepository.SaveChangesAsync();

        return BaseResult<RoleDto>.Success(mapper.Map<RoleDto>(role));
    }

    public async Task<BaseResult<RoleDto>> DeleteRoleAsync(long id)
    {
        var role = await roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
        if (role == null)
            return BaseResult<RoleDto>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

        var usersWithRoleToDelete = await GetUsersWithRoleAsync(role.Id);

        var areRolesUpdated = false;
        await using (var transaction = await unitOfWork.BeginTransactionAsync())
        {
            try
            {
                roleRepository.Remove(role);
                await roleRepository.SaveChangesAsync();

                await UpdateRolesAsync(usersWithRoleToDelete.Select(x => new User
                {
                    Id = x.Id,
                    KeycloakId = x.KeycloakId,
                    Email = x.Email,
                    Roles = x.Roles.Where(y => y.Id != role.Id).ToList()
                }));
                areRolesUpdated = true;

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                if (!areRolesUpdated) throw;

                RollbackRoles(usersWithRoleToDelete);

                throw;
            }
        }

        return BaseResult<RoleDto>.Success(mapper.Map<RoleDto>(role));
    }

    public async Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto)
    {
        var role = await roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (role == null)
            return BaseResult<RoleDto>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

        var areRolesUpdated = false;
        await using (var transaction = await unitOfWork.BeginTransactionAsync())
        {
            try
            {
                role.Name = dto.Name;
                roleRepository.Update(role);
                await roleRepository.SaveChangesAsync();

                var usersWithUpdatedRole = await GetUsersWithRoleAsync(role.Id);
                await UpdateRolesAsync(usersWithUpdatedRole);
                areRolesUpdated = true;

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                if (!areRolesUpdated) throw;

                var usersWithOldRole = await GetUsersWithRoleAsync(role.Id);
                RollbackRoles(usersWithOldRole);

                throw;
            }
        }

        return BaseResult<RoleDto>.Success(mapper.Map<RoleDto>(role));
    }

    public async Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto)
    {
        var user = await userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Username == dto.Username.ToLowerInvariant());

        if (user == null)
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var roles = user.Roles.Select(x => x.Id).ToArray();

        if (roles.Any(x => x == dto.RoleId))
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.UserAlreadyHasThisRole,
                (int)ErrorCodes.UserAlreadyHasThisRole);

        var role = await roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.RoleId);
        if (role == null)
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

        var areRolesUpdated = false;
        await using (var transaction = await unitOfWork.BeginTransactionAsync())
        {
            try
            {
                var userRole = new UserRole
                {
                    RoleId = role.Id,
                    UserId = user.Id
                };

                await userRoleRepository.CreateAsync(userRole);
                await userRoleRepository.SaveChangesAsync();

                var userWithUpdatedRole = await GetUserWithRolesByIdAsync(user.Id);
                await UpdateRolesAsync([userWithUpdatedRole!]);
                areRolesUpdated = true;

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                if (!areRolesUpdated) throw;

                var userWithOldRole = await GetUserWithRolesByIdAsync(user.Id);
                RollbackRoles(new[] { userWithOldRole! });

                throw;
            }
        }

        return BaseResult<UserRoleDto>.Success(new UserRoleDto
        {
            Username = user.Username,
            RoleId = role.Id
        });
    }

    public async Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(DeleteUserRoleDto dto)
    {
        var user = await userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Username == dto.Username.ToLowerInvariant());

        if (user == null)
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var role = user.Roles.FirstOrDefault(x => x.Id == dto.RoleId);

        if (role == null)
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

        var areRolesUpdated = false;
        await using (var transaction = await unitOfWork.BeginTransactionAsync())
        {
            try
            {
                var userRole = await userRoleRepository.GetAll()
                    .Where(x => x.RoleId == role.Id)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                userRoleRepository.Remove(userRole!);
                await userRoleRepository.SaveChangesAsync();

                var userWithDeletedRole = await GetUserWithRolesByIdAsync(user.Id);
                await UpdateRolesAsync([userWithDeletedRole!]);
                areRolesUpdated = true;

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                if (!areRolesUpdated) throw;

                var userWithOldRole = await GetUserWithRolesByIdAsync(user.Id);
                RollbackRoles(new[] { userWithOldRole! });

                throw;
            }
        }

        return BaseResult<UserRoleDto>.Success(new UserRoleDto
        {
            Username = user.Username,
            RoleId = role.Id
        });
    }

    public async Task<BaseResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto)
    {
        var user = await userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Username == dto.Username.ToLowerInvariant());

        if (user == null)
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var role = user.Roles.FirstOrDefault(x => x.Id == dto.FromRoleId);

        if (role == null)
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.RoleToBeUpdatedIsNotFound,
                (int)ErrorCodes.RoleNotFound);

        var newRoleForUser = await roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.ToRoleId);

        if (newRoleForUser == null)
            return BaseResult<UserRoleDto>.Failure(ErrorMessage.RoleToUpdateIsNotFound, (int)ErrorCodes.RoleNotFound);

        var areRolesUpdated = false;
        await using (var transaction = await unitOfWork.BeginTransactionAsync())
        {
            try
            {
                var newUserRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = newRoleForUser.Id
                };

                var isNewUserRoleExists = await unitOfWork.UserRoles.GetAll()
                    .FirstOrDefaultAsync(x => x.UserId == newUserRole.UserId && x.RoleId == newUserRole.RoleId) != null;

                if (isNewUserRoleExists)
                    return BaseResult<UserRoleDto>.Failure(ErrorMessage.UserAlreadyHasThisRole,
                        (int)ErrorCodes.UserAlreadyHasThisRole);

                var userRole = await unitOfWork.UserRoles.GetAll()
                    .Where(x => x.RoleId == role.Id)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                unitOfWork.UserRoles.Remove(userRole!);
                await unitOfWork.SaveChangesAsync();

                await unitOfWork.UserRoles.CreateAsync(newUserRole);
                await unitOfWork.SaveChangesAsync();

                var userWithUpdatedRole = await GetUserWithRolesByIdAsync(user.Id);
                await UpdateRolesAsync(new[] { userWithUpdatedRole! });
                areRolesUpdated = true;

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                if (!areRolesUpdated) throw;

                var userWithOldRole = await GetUserWithRolesByIdAsync(user.Id);
                RollbackRoles(new[] { userWithOldRole! });

                throw;
            }
        }

        return BaseResult<UserRoleDto>.Success(new UserRoleDto
        {
            Username = user.Username,
            RoleId = newRoleForUser.Id
        });
    }

    private async Task<User?> GetUserWithRolesByIdAsync(long userId)
    {
        return await userRepository.GetAll()
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }

    private async Task<User[]> GetUsersWithRoleAsync(long roleId)
    {
        return await userRepository.GetAll()
            .AsNoTracking()
            .Include(x => x.Roles)
            .Where(x => x.Roles.Any(y => y.Id == roleId))
            .ToArrayAsync();
    }

    private async Task UpdateRolesAsync(IEnumerable<User> users)
    {
        var updateTasks = users.Select(user =>
        {
            var dto = mapper.Map<KeycloakUpdateRolesDto>(user);
            return identityServer.UpdateRolesAsync(dto);
        });

        await Task.WhenAll(updateTasks);
    }

    private void RollbackRoles(IEnumerable<User> users)
    {
        foreach (var user in users)
        {
            var dto = mapper.Map<KeycloakUpdateRolesDto>(user);
            dto.NewRoles.ForEach(x => x.Users = null!); //Removing loop dependencies
            BackgroundJob.Enqueue(() => identityServer.RollbackUpdateRolesAsync(dto));
        }
    }
}