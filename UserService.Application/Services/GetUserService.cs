using Microsoft.EntityFrameworkCore;
using UserService.Application.Enums;
using UserService.Application.Extensions;
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
    IBaseRepository<ReputationRecord> reputationRecordRepository)
    : IGetUserService
{
    public QueryableResult<User> GetAll()
    {
        var users = userRepository.GetAll().AsNoTracking();

        return QueryableResult<User>.Success(users);
    }

    public async Task<CollectionResult<User>> GetByIdsAsync(IReadOnlyCollection<long> ids,
        CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAll().AsNoTracking().Where(x => ids.Contains(x.Id))
            .ToArrayAsync(cancellationToken);

        if (users.Length == 0) return CollectionResult<User>.UsersNotFound(ids.Count);

        return CollectionResult<User>.Success(users);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<User>>>> GetUsersWithRolesAsync(
        IReadOnlyCollection<long> roleIds, CancellationToken cancellationToken = default)
    {
        var groupedUsers = await roleRepository.GetAll()
            .AsNoTracking()
            .Where(x => roleIds.Contains(x.Id))
            .Include(x => x.Users)
            .Select(x => new KeyValuePair<long, IEnumerable<User>>(x.Id, x.Users.ToArray()))
            .ToArrayAsync(cancellationToken);

        if (groupedUsers.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Failure(ErrorMessage.UsersNotFound,
                (int)ErrorCodes.UsersNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Success(groupedUsers);
    }

    public async Task<CollectionResult<KeyValuePair<long, int>>> GetCurrentReputationsAsync(
        IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();

        var reputations = await reputationRecordRepository.GetAll()
            .AsNoTracking()
            .Where(x => idsArray.Contains(x.ReputationTargetId))
            .Include(x => x.ReputationRule)
            .GroupBy(x => new { UserId = x.ReputationTargetId, x.CreatedAt.Date })
            .Select(x => new KeyValuePair<long, int>(x.Key.UserId,
                Math.Max(BusinessRules.MinReputation,
                    Math.Min(x.Sum(y => y.ReputationRule.ReputationChange),
                        BusinessRules.MaxDailyReputation))))
            .ToArrayAsync(cancellationToken);

        var missingIds = idsArray.Except(reputations.Select(x => x.Key)).ToArray();
        KeyValuePair<long, int>[] missingReputations = [];
        if (missingIds.Length > 0)
            missingReputations = await userRepository.GetAll()
                .AsNoTracking()
                .Where(x => missingIds.Contains(x.Id))
                .Select(x => new KeyValuePair<long, int>(x.Id, BusinessRules.MinReputation))
                .ToArrayAsync(cancellationToken);


        var allReputations = reputations.Concat(missingReputations).ToArray();

        if (allReputations.Length == 0)
            return CollectionResult<KeyValuePair<long, int>>.UsersNotFound(idsArray.Length);

        return CollectionResult<KeyValuePair<long, int>>.Success(allReputations);
    }

    public async Task<CollectionResult<KeyValuePair<long, int>>> GetRemainingReputationsAsync(
        IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var reputations = await reputationRecordRepository.GetAll()
            .AsNoTracking()
            .Include(x => x.ReputationRule)
            .Where(x => idsArray.Contains(x.ReputationTargetId) && x.CreatedAt.Date == DateTime.UtcNow.Date &&
                        x.ReputationRule.ReputationChange > 0)
            .GroupBy(x => x.ReputationTargetId)
            .Select(x => new KeyValuePair<long, int>(x.Key,
                Math.Max(0, BusinessRules.MaxDailyReputation - x.Sum(y => y.ReputationRule.ReputationChange))))
            .ToArrayAsync(cancellationToken);

        var missingIds = idsArray.Except(reputations.Select(x => x.Key)).ToArray();
        KeyValuePair<long, int>[] missingReputations = [];
        if (missingIds.Length > 0)
            missingReputations = await userRepository.GetAll()
                .AsNoTracking()
                .Where(x => missingIds.Contains(x.Id))
                .Select(x => new KeyValuePair<long, int>(x.Id, BusinessRules.MaxDailyReputation))
                .ToArrayAsync(cancellationToken);

        var allReputations = reputations.Concat(missingReputations).ToArray();

        if (allReputations.Length == 0)
            return CollectionResult<KeyValuePair<long, int>>.UsersNotFound(idsArray.Length);

        return CollectionResult<KeyValuePair<long, int>>.Success(allReputations);
    }
}