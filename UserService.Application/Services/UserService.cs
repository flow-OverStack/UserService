using AutoMapper;
using FluentValidation;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Enums;
using UserService.Application.Helpers;
using UserService.Application.Resources;
using UserService.Domain.Dtos.Identity;
using UserService.Domain.Dtos.User;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services;

public class UserService(
    IMapper mapper,
    IIdentityServer identityServer,
    IUnitOfWork unitOfWork,
    IBackgroundJobClient backgroundJob,
    IValidator<UpdateUsernameDto> updateUsernameValidator)
    : IUserService
{
    public async Task<BaseResult<UserDto>> UpdateUsernameAsync(UpdateUsernameDto dto,
        CancellationToken cancellationToken = default)
    {
        dto = dto with { Username = dto.Username.ToLowerInvariant() };

        var validation = await updateUsernameValidator.ValidateWithMessageAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResult<UserDto>.Failure(validation.ErrorMessage, (int)ErrorCodes.InvalidProperty);

        var user = await unitOfWork.Users.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId, cancellationToken);
        if (user == null)
            return BaseResult<UserDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        if (user.Username == dto.Username)
            return BaseResult<UserDto>.Success(mapper.Map<UserDto>(user));

        var isTaken = await unitOfWork.Users.GetAll().AnyAsync(x => x.Username == dto.Username, cancellationToken);
        if (isTaken)
            return BaseResult<UserDto>.Failure(ErrorMessage.UsernameAlreadyTaken, (int)ErrorCodes.UsernameAlreadyTaken);

        IdentityUpdateUserDto? identityDto = null;
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            user.Username = dto.Username;
            await unitOfWork.SaveChangesAsync(cancellationToken);

            identityDto = mapper.Map<IdentityUpdateUserDto>(user);
            await identityServer.UpdateUserAsync(identityDto, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            if (identityDto != null)
                backgroundJob.Enqueue<IIdentityServer>(server =>
                    server.RollbackUpdateUserAsync(identityDto));

            throw;
        }

        return BaseResult<UserDto>.Success(mapper.Map<UserDto>(user));
    }
}