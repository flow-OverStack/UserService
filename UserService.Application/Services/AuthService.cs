using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Dto.Keycloak.User;
using UserService.Domain.Dto.Token;
using UserService.Domain.Dto.User;
using UserService.Domain.Entity;
using UserService.Domain.Enum;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Resources;
using UserService.Domain.Result;

namespace UserService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IIdentityServer _identityServer;
    private readonly IMapper _mapper;
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<UserToken> _userTokenRepository;

    public AuthService(IBaseRepository<User> userRepository, IMapper mapper,
        IIdentityServer identityServer,
        IBaseRepository<Role> roleRepository, IUnitOfWork unitOfWork,
        IBaseRepository<UserToken> userTokenRepository)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _identityServer = identityServer;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _userTokenRepository = userTokenRepository;
    }

    public async Task<BaseResult<UserDto>> Register(RegisterUserDto dto)
    {
        if (dto.Password != dto.PasswordConfirm)
            return new BaseResult<UserDto>
            {
                ErrorMessage = ErrorMessage.PasswordMismatch,
                ErrorCode = (int)ErrorCodes.PasswordMismatch
            };

        var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Username == dto.Username) ??
                   await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Email == dto.Email);
        if (user != null)
            return new BaseResult<UserDto>
            {
                ErrorMessage = ErrorMessage.UserAlreadyExists,
                ErrorCode = (int)ErrorCodes.UserAlreadyExists
            };

        var hashUserPassword = HashPassword(dto.Password);

        await using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = hashUserPassword
            };

            await _unitOfWork.Users.CreateAsync(user);

            await _unitOfWork.SaveChangesAsync();

            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == nameof(Roles.User));
            if (role == null)
                return new BaseResult<UserDto>
                {
                    ErrorMessage = ErrorMessage.RoleNotFound,
                    ErrorCode = (int)ErrorCodes.RoleNotFound
                };

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            await _unitOfWork.UserRoles.CreateAsync(userRole);

            await _unitOfWork.SaveChangesAsync();

            var keycloakDto = _mapper.Map<KeycloakRegisterUserDto>(user);

            var keycloakResponse = await _identityServer.RegisterUserAsync(keycloakDto);

            user.KeycloakId = keycloakResponse.KeycloakId;
            await _unitOfWork.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }


        return new BaseResult<UserDto>
        {
            Data = _mapper.Map<UserDto>(user)
        };
    }

    public async Task<BaseResult<TokenDto>> LoginWithUsername(LoginUsernameUserDto dto)
    {
        var user = await _userRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Username == dto.Username);

        return await Login(user, dto.Password);
    }

    public async Task<BaseResult<TokenDto>> LoginWithEmail(LoginEmailUserDto dto)
    {
        var user = await _userRepository.GetAll()
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        return await Login(user, dto.Password);
    }

    private async Task<BaseResult<TokenDto>> Login(User? user, string password)
    {
        if (user == null)
            return new BaseResult<TokenDto>
            {
                ErrorMessage = ErrorMessage.UserNotFound,
                ErrorCode = (int)ErrorCodes.UserNotFound
            };


        if (!IsVerifiedPassword(user.Password, password))
            return new BaseResult<TokenDto>
            {
                ErrorMessage = ErrorMessage.PasswordIsWrong,
                ErrorCode = (int)ErrorCodes.PasswordIsWrong
            };

        var userToken = await _userTokenRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);

        var keycloakResponse = await _identityServer.LoginUserAsync(_mapper.Map<KeycloakLoginUserDto>(user));

        if (userToken == null)
        {
            userToken = new UserToken
            {
                UserId = user.Id,
                RefreshToken = keycloakResponse.RefreshToken,
                RefreshTokenExpiryTime = keycloakResponse.Expires
            };

            await _userTokenRepository.CreateAsync(userToken);
        }
        else
        {
            userToken.RefreshToken = keycloakResponse.RefreshToken;
            userToken.RefreshTokenExpiryTime = keycloakResponse.Expires;

            _userTokenRepository.Update(userToken);
        }

        await _userTokenRepository.SaveChangesAsync();

        return new BaseResult<TokenDto>
        {
            Data = new TokenDto
            {
                AccessToken = keycloakResponse.AccessToken,
                RefreshToken = keycloakResponse.RefreshToken,
                Expires = keycloakResponse.Expires,
                UserId = user.Id
            }
        };
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static bool IsVerifiedPassword(string userPasswordHash, string userPassword)
    {
        var hash = HashPassword(userPassword);
        return userPasswordHash == hash;
    }
}