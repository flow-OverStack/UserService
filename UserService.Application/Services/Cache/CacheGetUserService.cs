using Microsoft.Extensions.Options;
using UserService.Cache.Repositories;
using UserService.Domain.Entities;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;
using UserService.Domain.Settings;
using Role = UserService.Domain.Entities.Role;

namespace UserService.Application.Services.Cache;

public class CacheGetUserService : IGetUserService
{
    private readonly IBaseCacheRepository<User, long> _cacheRepository;
    private readonly IGetUserService _inner;
    private readonly RedisSettings _redisSettings;
    private readonly IBaseCacheRepository<Role, long> _roleCacheRepository;
    private readonly IGetRoleService _roleInner;

    public CacheGetUserService(GetUserService inner, GetRoleService roleInner, ICacheProvider cacheProvider,
        IOptions<RedisSettings> redisSettings)
    {
        _cacheRepository = new BaseCacheRepository<User, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetUserKey,
            x => x.Id.ToString(),
            long.Parse
        );
        _roleCacheRepository = new BaseCacheRepository<Role, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetRoleKey,
            x => x.Id.ToString(),
            long.Parse
        );
        _inner = inner;
        _roleInner = roleInner;
        _redisSettings = redisSettings.Value;
    }

    public Task<QueryableResult<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _inner.GetAllAsync(cancellationToken);
    }

    public Task<CollectionResult<User>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        return _cacheRepository.GetByIdsOrFetchAndCacheAsync(
            ids,
            _inner.GetByIdsAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );
    }

    public async Task<BaseResult<User>> GetByIdWithRolesAsync(long id, CancellationToken cancellationToken = default)
    {
        var userResult = await _cacheRepository.GetByIdsOrFetchAndCacheAsync(
            [id],
            _inner.GetByIdsAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );

        if (!userResult.IsSuccess)
            return BaseResult<User>.Failure(userResult.ErrorMessage!, userResult.ErrorCode);

        var user = userResult.Data.Single();

        var rolesResult = await _roleCacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            [user.Id],
            CacheKeyHelper.GetUserRolesKey,
            CacheKeyHelper.GetIdFromKey,
            _roleInner.GetUsersRolesAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );

        if (!rolesResult.IsSuccess)
            return BaseResult<User>.Failure(userResult.ErrorMessage!, userResult.ErrorCode);

        var roles = rolesResult.Data.Single().Value;

        user.Roles = roles.ToList();

        return BaseResult<User>.Success(user);
    }

    public Task<CollectionResult<KeyValuePair<long, IEnumerable<User>>>> GetUsersWithRolesAsync(
        IEnumerable<long> roleIds, CancellationToken cancellationToken = default)
    {
        return _cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            roleIds,
            CacheKeyHelper.GetRoleUsersKey,
            CacheKeyHelper.GetIdFromKey,
            _inner.GetUsersWithRolesAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );
    }
}