using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserService.Application.Enums;
using UserService.Application.Resources;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;
using UserService.Domain.Settings;

namespace UserService.Application.Services;

public class GetUserService(
    IBaseRepository<User> userRepository,
    IBaseRepository<Role> roleRepository,
    IBaseRepository<ReputationRecord> reputationRecordRepository,
    IOptions<ReputationRules> reputationRules)
    : IGetUserService
{
    private readonly ReputationRules _reputationRules = reputationRules.Value;

    public Task<QueryableResult<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var users = userRepository.GetAll();

        return Task.FromResult(QueryableResult<User>.Success(users));
    }

    public async Task<CollectionResult<User>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAll().Where(x => ids.Contains(x.Id)).ToArrayAsync(cancellationToken);

        if (users.Length == 0)
            return ids.Count() switch
            {
                <= 1 => CollectionResult<User>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound),
                > 1 => CollectionResult<User>.Failure(ErrorMessage.UsersNotFound, (int)ErrorCodes.UsersNotFound)
            };

        return CollectionResult<User>.Success(users);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<User>>>> GetUsersWithRolesAsync(
        IEnumerable<long> roleIds, CancellationToken cancellationToken = default)
    {
        var groupedUsers = await roleRepository.GetAll()
            .Where(x => roleIds.Contains(x.Id))
            .Include(x => x.Users)
            .Select(x => new KeyValuePair<long, IEnumerable<User>>(x.Id, x.Users.ToArray()))
            .ToArrayAsync(cancellationToken);

        if (groupedUsers.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Failure(ErrorMessage.UsersNotFound,
                (int)ErrorCodes.UsersNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Success(groupedUsers);
    }

    public async Task<CollectionResult<KeyValuePair<long, int>>> GetCurrentReputationsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();

        var reputations = await reputationRecordRepository.GetAll()
            .Where(x => idsArray.Contains(x.UserId))
            .Include(x => x.ReputationRule)
            .GroupBy(x => new { x.UserId, x.CreatedAt.Date })
            .Select(x => new KeyValuePair<long, int>(x.Key.UserId,
                Math.Max(_reputationRules.MinReputation,
                    Math.Min(x.Sum(y => y.ReputationRule.ReputationChange),
                        _reputationRules.MaxDailyReputation))))
            .ToArrayAsync(cancellationToken);

        var missingIds = idsArray.Except(reputations.Select(x => x.Key)).ToArray();
        KeyValuePair<long, int>[] missingReputations = [];
        if (missingIds.Length > 0)
            missingReputations = await userRepository.GetAll()
                .Where(x => missingIds.Contains(x.Id))
                .Select(x => new KeyValuePair<long, int>(x.Id, _reputationRules.MinReputation))
                .ToArrayAsync(cancellationToken);


        var allReputations = reputations.Concat(missingReputations).ToArray();

        if (allReputations.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<KeyValuePair<long, int>>.Failure(ErrorMessage.UserNotFound,
                    (int)ErrorCodes.UserNotFound),
                > 1 => CollectionResult<KeyValuePair<long, int>>.Failure(ErrorMessage.UsersNotFound,
                    (int)ErrorCodes.UsersNotFound)
            };

        return CollectionResult<KeyValuePair<long, int>>.Success(allReputations);
    }

    public async Task<CollectionResult<KeyValuePair<long, int>>> GetRemainingReputationsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var reputations = await reputationRecordRepository.GetAll()
            .Include(x => x.ReputationRule)
            .Where(x => idsArray.Contains(x.UserId) && x.CreatedAt.Date == DateTime.UtcNow.Date &&
                        x.ReputationRule.ReputationChange > 0)
            .GroupBy(x => x.UserId)
            .Select(x => new KeyValuePair<long, int>(x.Key,
                Math.Max(0, _reputationRules.MaxDailyReputation - x.Sum(y => y.ReputationRule.ReputationChange))))
            .ToArrayAsync(cancellationToken);

        var missingIds = idsArray.Except(reputations.Select(x => x.Key)).ToArray();
        KeyValuePair<long, int>[] missingReputations = [];
        if (missingIds.Length > 0)
            missingReputations = await userRepository.GetAll()
                .Where(x => missingIds.Contains(x.Id))
                .Select(x => new KeyValuePair<long, int>(x.Id, _reputationRules.MaxDailyReputation))
                .ToArrayAsync(cancellationToken);

        var allReputations = reputations.Concat(missingReputations).ToArray();

        if (allReputations.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<KeyValuePair<long, int>>.Failure(ErrorMessage.UserNotFound,
                    (int)ErrorCodes.UserNotFound),
                > 1 => CollectionResult<KeyValuePair<long, int>>.Failure(ErrorMessage.UsersNotFound,
                    (int)ErrorCodes.UsersNotFound)
            };

        return CollectionResult<KeyValuePair<long, int>>.Success(allReputations);
    }
}