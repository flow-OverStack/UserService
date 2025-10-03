using AutoMapper;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services;

public class UserActivityService(
    IUserActivityCacheRepository cacheRepository,
    IBaseRepository<User> userRepository,
    IMapper mapper)
    : IUserActivityService, IUserActivityDatabaseService
{
    public async Task<BaseResult<SyncedHeartbeatsDto>> SyncHeartbeatsToDatabaseAsync(
        CancellationToken cancellationToken = default)
    {
        var activities = await cacheRepository.GetValidActivitiesAsync(cancellationToken);

        var users = activities.Select(mapper.Map<User>).ToArray();

        var propertiesToUpdate = typeof(UserActivityDto).GetProperties()
            .Select(p => p.Name == nameof(UserActivityDto.UserId) ? nameof(User.Id) : p.Name);
        await userRepository.BulkUpdateAsync(users, propertiesToUpdate, cancellationToken);

        await cacheRepository.DeleteAllActivitiesAsync(cancellationToken);

        return BaseResult<SyncedHeartbeatsDto>.Success(new SyncedHeartbeatsDto(users.Length));
    }

    public async Task<BaseResult> RegisterHeartbeatAsync(long id, CancellationToken cancellationToken = default)
    {
        // We do not check user existence
        // because that may increase the request processing time
        // And this request is called frequently

        var dto = new UserActivityDto(id, DateTime.UtcNow);
        await cacheRepository.AddActivityAsync(dto, cancellationToken);

        return BaseResult.Success();
    }
}