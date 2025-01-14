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

public class RoleService : IRoleService
{
    private readonly IIdentityServer _identityServer;
    private readonly IMapper _mapper;
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<UserRole> _userRoleRepository;

    public RoleService(IBaseRepository<User> userRepository, IBaseRepository<Role> roleRepository, IMapper mapper,
        IBaseRepository<UserRole> userRoleRepository, IUnitOfWork unitOfWork, IIdentityServer identityServer)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
        _identityServer = identityServer;
        throw new NotImplementedException();
    }

    public async Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto)
    {
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
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
        await _roleRepository.CreateAsync(role);
        await _roleRepository.SaveChangesAsync();
        return new BaseResult<RoleDto>
        {
            Data = _mapper.Map<RoleDto>(role)
        };
    }

    public async Task<BaseResult<RoleDto>> DeleteRoleAsync(long id)
    {
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
        if (role == null)
            return new BaseResult<RoleDto>
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };

        _roleRepository.Remove(role);
        await _roleRepository.SaveChangesAsync();
        return new BaseResult<RoleDto>
        {
            Data = _mapper.Map<RoleDto>(role)
        };
    }

    public async Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto)
    {
        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (role == null)
            return new BaseResult<RoleDto>
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };

        role.Name = dto.Name;
        var updatedRole = _roleRepository.Update(role);
        await _roleRepository.SaveChangesAsync();
        return new BaseResult<RoleDto>
        {
            Data = _mapper.Map<RoleDto>(updatedRole)
        };
    }

    public async Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto)
    {
        var user = await _userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Username == dto.Username);

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

        var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.RoleName);
        if (role == null)
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = ErrorMessage.RoleNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };

        await using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                var userRole = new UserRole
                {
                    RoleId = role.Id,
                    UserId = user.Id
                };

                await _userRoleRepository.CreateAsync(userRole);
                await _userRoleRepository.SaveChangesAsync();

                var userWithUpdatedRoles = await _userRepository.GetAll()
                    .AsNoTracking()
                    .Include(x => x.Roles)
                    .Select(x => new { x.Id, x.Roles })
                    .FirstOrDefaultAsync(x => x.Id == user.Id);

                await _identityServer.UpdateRolesAsync(new KeycloakUpdateRolesDto(user.KeycloakId,
                    userWithUpdatedRoles!.Roles));

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
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
        var user = await _userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Username == dto.Username);

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

        await using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                var userRole = await _userRoleRepository.GetAll()
                    .Where(x => x.RoleId == role.Id)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                _userRoleRepository.Remove(userRole!);
                await _userRoleRepository.SaveChangesAsync();

                var userWithUpdatedRoles = await _userRepository.GetAll()
                    .AsNoTracking()
                    .Include(x => x.Roles)
                    .Select(x => new { x.Id, x.Roles })
                    .FirstOrDefaultAsync(x => x.Id == user.Id);

                await _identityServer.UpdateRolesAsync(new KeycloakUpdateRolesDto(user.KeycloakId,
                    userWithUpdatedRoles!.Roles));

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
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
        var user = await _userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Username == dto.Username);

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

        var newRoleForUser = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.ToRoleId);

        if (newRoleForUser == null)
            return new BaseResult<UserRoleDto>
            {
                ErrorMessage = ErrorMessage.RoleToUpdateIsNotFound,
                ErrorCode = (int)ErrorCodes.RoleNotFound
            };

        await using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                var newUserRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = newRoleForUser.Id
                };

                var isNewUserRoleExists = await _unitOfWork.UserRoles.GetAll()
                    .FirstOrDefaultAsync(x => x.UserId == newUserRole.UserId && x.RoleId == newUserRole.RoleId) != null;

                if (isNewUserRoleExists)
                    return new BaseResult<UserRoleDto>
                    {
                        ErrorMessage = ErrorMessage.UserAlreadyHasThisRole,
                        ErrorCode = (int)ErrorCodes.UserAlreadyHasThisRole
                    };

                var userRole = await _unitOfWork.UserRoles.GetAll()
                    .Where(x => x.RoleId == role.Id)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                _unitOfWork.UserRoles.Remove(userRole!);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.UserRoles.CreateAsync(newUserRole);
                await _unitOfWork.SaveChangesAsync();

                var userWithUpdatedRoles = await _userRepository.GetAll()
                    .AsNoTracking()
                    .Include(x => x.Roles)
                    .Select(x => new { x.Id, x.Roles })
                    .FirstOrDefaultAsync(x => x.Id == user.Id);

                await _identityServer.UpdateRolesAsync(new KeycloakUpdateRolesDto(user.KeycloakId,
                    userWithUpdatedRoles!.Roles));

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
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
}