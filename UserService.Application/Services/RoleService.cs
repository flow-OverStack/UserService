using AutoMapper;
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
            return new BaseResult<RoleDto>
            {
                ErrorMessage = ErrorMessage.RoleAlreadyExists,
                ErrorCode = (int)ErrorCodes.RoleAlreadyExists
            };

        role = new Role
        {
            Name = dto.Name
        };
        await roleRepository.CreateAsync(role);
        await roleRepository.SaveChangesAsync();

        return new BaseResult<RoleDto>
        {
            Data = mapper.Map<RoleDto>(role)
        };
    }

    public async Task<BaseResult<RoleDto>> DeleteRoleAsync(long id)
    {
        var role = await roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
        if (role == null)
            return new BaseResult<RoleDto>
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };

        var usersWithRoleToDelete = await GetUsersWithRoleAsync(role.Id);

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

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                await RollbackRolesAsync(usersWithRoleToDelete);

                throw;
            }
        }

        return new BaseResult<RoleDto>
        {
            Data = mapper.Map<RoleDto>(role)
        };
    }

    public async Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto)
    {
        var role = await roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (role == null)
            return new BaseResult<RoleDto>
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };

        await using (var transaction = await unitOfWork.BeginTransactionAsync())
        {
            try
            {
                role.Name = dto.Name;
                roleRepository.Update(role);
                await roleRepository.SaveChangesAsync();

                var usersWithUpdatedRole = await GetUsersWithRoleAsync(role.Id);
                await UpdateRolesAsync(usersWithUpdatedRole);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                var usersWithOldRole = await GetUsersWithRoleAsync(role.Id);
                await RollbackRolesAsync(usersWithOldRole);

                throw;
            }
        }

        return new BaseResult<RoleDto>
        {
            Data = mapper.Map<RoleDto>(role)
        };
    }

    public async Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto)
    {
        var user = await userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Username == dto.Username.ToLowerInvariant());

        if (user == null)
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = ErrorMessage.UserNotFound,
                ErrorCode = (int)ErrorCodes.UserNotFound
            };

        var roles = user.Roles.Select(x => x.Name).ToArray();

        if (roles.Any(x => x == dto.RoleName))
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = ErrorMessage.UserAlreadyHasThisRole,
                ErrorCode = (int)ErrorCodes.UserAlreadyHasThisRole
            };

        var role = await roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.RoleName);
        if (role == null)
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };

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

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                var userWithOldRole = await GetUserWithRolesByIdAsync(user.Id);
                await RollbackRolesAsync([userWithOldRole!]);

                throw;
            }
        }

        return new BaseResult<UserRoleDto>
        {
            Data = new UserRoleDto
            {
                Username = user.Username,
                RoleName = role.Name
            }
        };
    }

    public async Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(DeleteUserRoleDto dto)
    {
        var user = await userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Username == dto.Username.ToLowerInvariant());

        if (user == null)
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = ErrorMessage.UserNotFound,
                ErrorCode = (int)ErrorCodes.UserNotFound
            };

        var role = user.Roles.FirstOrDefault(x => x.Id == dto.RoleId);

        if (role == null)
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };

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

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                var userWithOldRole = await GetUserWithRolesByIdAsync(user.Id);
                await RollbackRolesAsync([userWithOldRole!]);

                throw;
            }
        }

        return new BaseResult<UserRoleDto>
        {
            Data = new UserRoleDto
            {
                Username = user.Username,
                RoleName = role.Name
            }
        };
    }

    public async Task<BaseResult<UserRoleDto>> UpdateRoleForUserAsync(UpdateUserRoleDto dto)
    {
        var user = await userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Username == dto.Username.ToLowerInvariant());

        if (user == null)
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = ErrorMessage.UserNotFound,
                ErrorCode = (int)ErrorCodes.UserNotFound
            };

        var role = user.Roles.FirstOrDefault(x => x.Id == dto.FromRoleId);

        if (role == null)
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = ErrorMessage.RoleToBeUpdatedIsNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };

        var newRoleForUser = await roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.ToRoleId);

        if (newRoleForUser == null)
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = ErrorMessage.RoleToUpdateIsNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };

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
                    return new BaseResult<UserRoleDto>
                    {
                        ErrorMessage = ErrorMessage.UserAlreadyHasThisRole,
                        ErrorCode = (int)ErrorCodes.UserAlreadyHasThisRole
                    };

                var userRole = await unitOfWork.UserRoles.GetAll()
                    .Where(x => x.RoleId == role.Id)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                unitOfWork.UserRoles.Remove(userRole!);
                await unitOfWork.SaveChangesAsync();

                await unitOfWork.UserRoles.CreateAsync(newUserRole);
                await unitOfWork.SaveChangesAsync();

                var userWithUpdatedRole = await GetUserWithRolesByIdAsync(user.Id);
                await UpdateRolesAsync([userWithUpdatedRole!]);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                var userWithOldRole = await GetUserWithRolesByIdAsync(user.Id);
                await RollbackRolesAsync([userWithOldRole!]);

                throw;
            }
        }

        return new BaseResult<UserRoleDto>
        {
            Data = new UserRoleDto
            {
                Username = user.Username,
                RoleName = newRoleForUser.Name
            }
        };
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

    private async Task RollbackRolesAsync(IEnumerable<User> users)
    {
        var rollbackTasks = users.Select(user =>
        {
            var dto = mapper.Map<KeycloakUpdateRolesDto>(user);
            return identityServer.RollbackUpdateRolesAsync(dto);
        });

        await Task.WhenAll(rollbackTasks);
    }
}