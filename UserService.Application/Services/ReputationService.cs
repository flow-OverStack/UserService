using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserService.Domain.Dto.User;
using UserService.Domain.Entity;
using UserService.Domain.Enum;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Resources;
using UserService.Domain.Result;
using UserService.Domain.Settings;

namespace UserService.Application.Services;

public class ReputationService(IBaseRepository<User> userRepository, IOptions<BusinessRules> businessRules)
    : IReputationService, IReputationResetService
{
    private readonly BusinessRules _businessRules = businessRules.Value;

    public async Task<BaseResult> ResetEarnedTodayReputationAsync()
    {
        await userRepository.GetAll().ExecuteUpdateAsync(x => x.SetProperty(y => y.ReputationEarnedToday, 0));

        return BaseResult.Success();
    }

    public async Task<BaseResult<ReputationDto>> IncreaseReputationAsync(ReputationIncreaseDto dto)
    {
        if (dto.ReputationToIncrease < 0)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.CannotIncreaseOrDecreaseNegativeReputation,
                (int)ErrorCodes.CannotIncreaseOrDecreaseNegativeReputation);

        var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId);

        if (user == null)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var reputationToIncrease = CalculateReputationToIncrease(user.ReputationEarnedToday, dto.ReputationToIncrease);
        if (reputationToIncrease == 0)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.DailyReputationLimitExceeded,
                (int)ErrorCodes.DailyReputationLimitExceeded);

        user.Reputation += reputationToIncrease;
        user.ReputationEarnedToday += reputationToIncrease;

        await userRepository.SaveChangesAsync();

        var remainingDailyLimit = _businessRules.MaxDailyReputation - user.ReputationEarnedToday;

        return BaseResult<ReputationDto>.Success(new ReputationDto
        {
            UserId = dto.UserId,
            CurrentReputation = user.Reputation,
            RemainingDailyLimit = remainingDailyLimit
        });
    }

    public async Task<BaseResult<ReputationDto>> DecreaseReputationAsync(ReputationDecreaseDto dto)
    {
        if (dto.ReputationToDecrease < 0)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.CannotIncreaseOrDecreaseNegativeReputation,
                (int)ErrorCodes.CannotIncreaseOrDecreaseNegativeReputation);

        var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId);

        if (user == null)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var reputationToDecrease = CalculateReputationToDecrease(user.Reputation, dto.ReputationToDecrease);
        if (reputationToDecrease == 0)
            return BaseResult<ReputationDto>.Failure(ErrorMessage.ReputationMinimumReached,
                (int)ErrorCodes.ReputationMinimumReached);

        user.Reputation -= reputationToDecrease;

        await userRepository.SaveChangesAsync();

        var remainingDailyLimit = _businessRules.MaxDailyReputation - user.ReputationEarnedToday;

        return BaseResult<ReputationDto>.Success(new ReputationDto
        {
            UserId = user.Id,
            CurrentReputation = user.Reputation,
            RemainingDailyLimit = remainingDailyLimit
        });
    }

    private int CalculateReputationToIncrease(int reputationEarnedToday, int reputationToIncrease)
    {
        var remainingReputation = _businessRules.MaxDailyReputation - reputationEarnedToday;
        if (remainingReputation > 0)
            return Math.Min(remainingReputation, reputationToIncrease);

        return 0;
    }

    private int CalculateReputationToDecrease(int currentReputation, int reputationToDecrease)
    {
        var newReputation = currentReputation - reputationToDecrease;
        if (newReputation < _businessRules.MinReputation)
            return currentReputation - _businessRules.MinReputation;

        return reputationToDecrease;
    }
}