using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserService.Application.Enums;
using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;
using UserService.Domain.Settings;

namespace UserService.Application.Services;

public class ReputationService(IBaseRepository<User> userRepository, IOptions<ReputationRules> reputationRules)
    : IReputationService, IReputationResetService
{
    private readonly ReputationRules _reputationRules = reputationRules.Value;

    public async Task<BaseResult> ResetEarnedTodayReputationAsync(CancellationToken cancellationToken = default)
    {
        await userRepository.GetAll()
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.ReputationEarnedToday, 0), cancellationToken);

        return BaseResult.Success();
    }

    public async Task<BaseResult<ReputationDto>> IncreaseReputationAsync(ReputationIncreaseDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.ReputationToIncrease < 0)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.CannotIncreaseOrDecreaseNegativeReputation,
                (int)ErrorCodes.CannotIncreaseOrDecreaseNegativeReputation);

        var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId, cancellationToken);

        if (user == null)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var reputationToIncrease = CalculateReputationToIncrease(user.ReputationEarnedToday, dto.ReputationToIncrease);
        if (reputationToIncrease == 0)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.DailyReputationLimitExceeded,
                (int)ErrorCodes.DailyReputationLimitExceeded);

        user.Reputation += reputationToIncrease;
        user.ReputationEarnedToday += reputationToIncrease;

        await userRepository.SaveChangesAsync(cancellationToken);

        var remainingDailyLimit = _reputationRules.MaxDailyReputation - user.ReputationEarnedToday;

        return BaseResult<ReputationDto>.Success(new ReputationDto
        {
            UserId = dto.UserId,
            CurrentReputation = user.Reputation,
            RemainingDailyLimit = remainingDailyLimit
        });
    }

    public async Task<BaseResult<ReputationDto>> DecreaseReputationAsync(ReputationDecreaseDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.ReputationToDecrease < 0)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.CannotIncreaseOrDecreaseNegativeReputation,
                (int)ErrorCodes.CannotIncreaseOrDecreaseNegativeReputation);

        var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId, cancellationToken);

        if (user == null)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var reputationToDecrease = CalculateReputationToDecrease(user.Reputation, dto.ReputationToDecrease);
        if (reputationToDecrease == 0)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.ReputationMinimumReached,
                (int)ErrorCodes.ReputationMinimumReached);

        user.Reputation -= reputationToDecrease;

        await userRepository.SaveChangesAsync(cancellationToken);

        var remainingDailyLimit = _reputationRules.MaxDailyReputation - user.ReputationEarnedToday;

        return BaseResult<ReputationDto>.Success(new ReputationDto
        {
            UserId = user.Id,
            CurrentReputation = user.Reputation,
            RemainingDailyLimit = remainingDailyLimit
        });
    }

    private int CalculateReputationToIncrease(int reputationEarnedToday, int reputationToIncrease)
    {
        var remainingReputation = _reputationRules.MaxDailyReputation - reputationEarnedToday;
        if (remainingReputation > 0)
            return Math.Min(remainingReputation, reputationToIncrease);

        return 0;
    }

    private int CalculateReputationToDecrease(int currentReputation, int reputationToDecrease)
    {
        var newReputation = currentReputation - reputationToDecrease;
        if (newReputation < _reputationRules.MinReputation)
            return currentReputation - _reputationRules.MinReputation;

        return reputationToDecrease;
    }
}